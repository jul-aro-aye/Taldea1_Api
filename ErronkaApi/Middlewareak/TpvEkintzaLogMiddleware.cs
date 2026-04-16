using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ErronkaApi.Logak;
using Microsoft.AspNetCore.Http;

namespace ErronkaApi.Middlewareak
{
    public class TpvEkintzaLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Log _log;

        public TpvEkintzaLogMiddleware(RequestDelegate next, Log log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            if (EzDaLogeatuBeharrekoa(context.Request.Path))
            {
                await _next(context);
                return;
            }

            string body = await IrakurriBodya(context.Request);
            string erabiltzailea = LortuErabiltzailea(context, body);

            try
            {
                await _next(context);
                string mezua = SortuLogMezua(context, body, context.Response.StatusCode);
                _log.GordeLog(erabiltzailea, mezua);
            }
            catch (Exception ex)
            {
                string errorea = SortuErroreMezua(context, body, ex);
                _log.GordeLog(erabiltzailea, errorea);
                throw;
            }
        }

        private static bool EzDaLogeatuBeharrekoa(PathString path)
        {
            return path.StartsWithSegments("/swagger")
                   || path.StartsWithSegments("/TPV_Logs")
                   || path.StartsWithSegments("/favicon.ico");
        }

        private static async Task<string> IrakurriBodya(HttpRequest request)
        {
            if (request.ContentLength is null or 0 || request.Body == null || !request.Body.CanRead)
                return string.Empty;

            request.EnableBuffering();
            request.Body.Position = 0;

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            string body = await reader.ReadToEndAsync();

            request.Body.Position = 0;
            return body;
        }

        private static string LortuErabiltzailea(HttpContext context, string body)
        {
            string? erabiltzaileaHeader = LortuHeaderretik(context, "X-TPV-User", "X-Erabiltzailea", "X-User");
            if (!string.IsNullOrWhiteSpace(erabiltzaileaHeader))
                return erabiltzaileaHeader;

            if (context.Request.Query.TryGetValue("erabiltzailea", out var erabiltzaileaQuery))
            {
                string queryBalioa = erabiltzaileaQuery.ToString();
                if (!string.IsNullOrWhiteSpace(queryBalioa))
                    return queryBalioa;
            }

            string? erabiltzaileaBody = LortuErabiltzaileaBodytik(body);
            if (!string.IsNullOrWhiteSpace(erabiltzaileaBody))
                return erabiltzaileaBody;

            return "ezezaguna";
        }

        private static string? LortuHeaderretik(HttpContext context, params string[] headerIzenak)
        {
            foreach (string headerIzena in headerIzenak)
            {
                if (context.Request.Headers.TryGetValue(headerIzena, out var balioa))
                {
                    string testua = balioa.ToString();
                    if (!string.IsNullOrWhiteSpace(testua))
                        return testua;
                }
            }

            return null;
        }

        private static string SortuLogMezua(HttpContext context, string body, int statusCode)
        {
            string oinarria = SortuEkintzaMezua(context, body, statusCode);

            if (statusCode >= 500)
                return $"Errore larria ({statusCode}). {oinarria}";

            if (statusCode >= 400 && !DaEsperoDenNotFound(context, statusCode))
                return $"Ekintza ezin izan da osatu ({statusCode}). {oinarria}";

            return oinarria;
        }

        private static string SortuErroreMezua(HttpContext context, string body, Exception ex)
        {
            string oinarria = SortuEkintzaMezua(context, body, 500);
            return $"Errore larria (500). {oinarria} ({ex.GetType().Name})";
        }

        private static string SortuEkintzaMezua(HttpContext context, string body, int statusCode)
        {
            string method = context.Request.Method.ToUpperInvariant();
            string[] segmentuak = LortuPathSegmentuak(context.Request.Path);

            if (segmentuak.Length < 2 || !string.Equals(segmentuak[0], "api", StringComparison.OrdinalIgnoreCase))
                return $"{method} {context.Request.Path} egin da ({statusCode})";

            string baliabidea = segmentuak[1].ToLowerInvariant();
            JsonElement? bodyJson = ParseBodyJson(body);

            return baliabidea switch
            {
                "login" => SortuLoginMezua(method, segmentuak, bodyJson),
                "mahaiak" => SortuMahaiMezua(method, segmentuak, context, statusCode),
                "eskaerak" => SortuEskaeraMezua(method, segmentuak, context, bodyJson, statusCode),
                "fakturak" => SortuFakturaMezua(method, segmentuak, bodyJson),
                "produktuak" => SortuCrudMezua("Produktuak", method, segmentuak),
                "kategoriak" => SortuCrudMezua("Kategoriak", method, segmentuak),
                "erreserbak" => SortuErreserbaMezua(method, segmentuak),
                "erreserbamahaiak" => SortuErreserbaMahaiMezua(method, segmentuak),
                _ => $"{method} {context.Request.Path} egin da ({statusCode})"
            };
        }

        private static string SortuLoginMezua(string method, string[] segmentuak, JsonElement? bodyJson)
        {
            if (method == "POST")
            {
                string erabiltzailea = LortuString(bodyJson, "erabiltzailea", "username") ?? "ezezaguna";
                return $"Saioa hasi da. Erabiltzailea: {erabiltzailea}";
            }

            if (method == "GET" && segmentuak.Length >= 4 && segmentuak[3].Equals("txat", StringComparison.OrdinalIgnoreCase))
                return $"Txat baimena kontsultatuta. Erabiltzailea ID: {segmentuak[2]}";

            return "Login ekintza eginda";
        }

        private static string SortuMahaiMezua(string method, string[] segmentuak, HttpContext context, int statusCode)
        {
            if (method == "GET" && segmentuak.Length == 2)
            {
                string xehetasunak = LortuDataTxanda(context);
                return string.IsNullOrWhiteSpace(xehetasunak)
                    ? "Mahai zerrenda kontsultatuta"
                    : $"Mahai zerrenda kontsultatuta ({xehetasunak})";
            }

            if (method == "GET" && segmentuak.Length >= 3 && segmentuak[2].Equals("libre", StringComparison.OrdinalIgnoreCase))
                return statusCode == 404 ? "Ez dago mahai librerik" : "Mahai libreak kontsultatuta";

            if (method == "GET" && segmentuak.Length >= 3)
            {
                string xehetasunak = LortuDataTxanda(context);
                return string.IsNullOrWhiteSpace(xehetasunak)
                    ? $"Mahaia aukeratua. Mahaia ID: {segmentuak[2]}"
                    : $"Mahaia aukeratua. Mahaia ID: {segmentuak[2]} ({xehetasunak})";
            }

            return "Mahai ekintza eginda";
        }

        private static string SortuEskaeraMezua(
            string method,
            string[] segmentuak,
            HttpContext context,
            JsonElement? bodyJson,
            int statusCode)
        {
            if (method == "POST" && segmentuak.Length == 2)
            {
                string? mahaiaId = LortuInt(bodyJson, "MahaiaId", "mahaiaId")?.ToString();
                string? komentsalak = LortuInt(bodyJson, "Komensalak", "komensalak")?.ToString();
                string? produktuak = LortuProduktuenLaburpena(bodyJson);

                var zatiak = new List<string>();
                if (!string.IsNullOrWhiteSpace(mahaiaId)) zatiak.Add($"Mahaia: {mahaiaId}");
                if (!string.IsNullOrWhiteSpace(komentsalak)) zatiak.Add($"Komentsalak: {komentsalak}");
                if (!string.IsNullOrWhiteSpace(produktuak)) zatiak.Add($"Produktuak: {produktuak}");

                string xehetasunak = string.Join(", ", zatiak);
                return string.IsNullOrWhiteSpace(xehetasunak)
                    ? "Eskaera gorde da"
                    : $"Eskaera gorde da. {xehetasunak}";
            }

            if (method == "GET" && segmentuak.Length == 2)
                return "Eskaera aktiboak kontsultatuta";

            if (method == "GET" && segmentuak.Length >= 5 &&
                segmentuak[2].Equals("mahaia", StringComparison.OrdinalIgnoreCase) &&
                segmentuak[4].Equals("aktiboa", StringComparison.OrdinalIgnoreCase))
            {
                return statusCode == 404
                    ? $"Ez dago eskaera aktiborik. Mahaia: {segmentuak[3]}"
                    : $"Eskaera aktiboa aukeratua. Mahaia: {segmentuak[3]}";
            }

            if (method == "GET" && segmentuak.Length >= 4 &&
                segmentuak[3].Equals("produktuak", StringComparison.OrdinalIgnoreCase))
            {
                return $"Eskaeraren produktuak kontsultatuta. Eskaera ID: {segmentuak[2]}";
            }

            if (method == "GET" && segmentuak.Length >= 5 &&
                segmentuak[2].Equals("mahaiak", StringComparison.OrdinalIgnoreCase) &&
                segmentuak[4].Equals("kapazitatea", StringComparison.OrdinalIgnoreCase))
            {
                return $"Mahaiko kapazitatea kontsultatuta. Mahaia ID: {segmentuak[3]}";
            }

            if (method == "GET" && segmentuak.Length >= 3 &&
                segmentuak[2].Equals("ordainketa-pendiente", StringComparison.OrdinalIgnoreCase))
            {
                return "Ordainketa pendiente dauden eskaerak kontsultatuta";
            }

            if (method == "DELETE" && segmentuak.Length >= 3)
                return $"Eskaera ezabatu da. Eskaera ID: {segmentuak[2]}";

            if (method == "PUT" && segmentuak.Length >= 4 &&
                segmentuak[3].Equals("sukaldea-egoera", StringComparison.OrdinalIgnoreCase))
            {
                string? egoera = LortuString(bodyJson, "SukaldeaEgoera", "sukaldeaEgoera");
                return string.IsNullOrWhiteSpace(egoera)
                    ? $"Sukaldeko egoera eguneratu da. Eskaera ID: {segmentuak[2]}"
                    : $"Sukaldeko egoera eguneratu da. Eskaera ID: {segmentuak[2]}, Egoera: {egoera}";
            }

            if (method == "PUT" && segmentuak.Length >= 3)
            {
                string? komentsalak = LortuInt(bodyJson, "Komensalak", "komensalak")?.ToString();
                string? produktuak = LortuProduktuenLaburpena(bodyJson);

                var zatiak = new List<string> { $"Eskaera ID: {segmentuak[2]}" };
                if (!string.IsNullOrWhiteSpace(komentsalak)) zatiak.Add($"Komentsalak: {komentsalak}");
                if (!string.IsNullOrWhiteSpace(produktuak)) zatiak.Add($"Produktuak: {produktuak}");

                return $"Eskaera eguneratu da. {string.Join(", ", zatiak)}";
            }

            if (method == "POST" && segmentuak.Length >= 4 &&
                segmentuak[3].Equals("ordainduEskaera", StringComparison.OrdinalIgnoreCase))
            {
                return $"Eskaera ordainketara bidali da. Eskaera ID: {segmentuak[2]}";
            }

            if (method == "POST" && segmentuak.Length >= 4 &&
                segmentuak[3].Equals("sortuFaktura", StringComparison.OrdinalIgnoreCase))
            {
                return $"Faktura sortuta. Eskaera ID: {segmentuak[2]}";
            }

            return "Eskaera ekintza eginda";
        }

        private static string SortuFakturaMezua(string method, string[] segmentuak, JsonElement? bodyJson)
        {
            if (method == "GET" && segmentuak.Length == 2)
                return "Faktura zerrenda kontsultatuta";

            if (method == "GET" && segmentuak.Length >= 4 &&
                segmentuak[2].Equals("erreserba", StringComparison.OrdinalIgnoreCase))
                return $"Erreserbaren faktura kontsultatuta. Erreserba ID: {segmentuak[3]}";

            if (method == "GET" && segmentuak.Length >= 3)
                return $"Faktura kontsultatuta. Faktura ID: {segmentuak[2]}";

            if (method == "POST" && segmentuak.Length >= 3 &&
                segmentuak[2].Equals("sortu-erreserbatik", StringComparison.OrdinalIgnoreCase))
            {
                string? erreserbaId = LortuInt(bodyJson, "ErreserbaId", "erreserbaId")?.ToString();
                return string.IsNullOrWhiteSpace(erreserbaId)
                    ? "Faktura sortu edo berreskuratu da erreserbatik"
                    : $"Faktura sortu edo berreskuratu da erreserbatik. Erreserba ID: {erreserbaId}";
            }

            if (method == "POST" && segmentuak.Length >= 3 &&
                segmentuak[2].Equals("eguneratu-totala", StringComparison.OrdinalIgnoreCase))
            {
                string? fakturaId = LortuInt(bodyJson, "FakturaId", "fakturaId")?.ToString();
                string? gehikuntza = LortuDecimal(bodyJson, "Gehikuntza", "gehikuntza");

                var zatiak = new List<string>();
                if (!string.IsNullOrWhiteSpace(fakturaId)) zatiak.Add($"Faktura ID: {fakturaId}");
                if (!string.IsNullOrWhiteSpace(gehikuntza)) zatiak.Add($"Gehikuntza: {gehikuntza}");

                return zatiak.Count == 0
                    ? "Fakturaren totala eguneratu da"
                    : $"Fakturaren totala eguneratu da. {string.Join(", ", zatiak)}";
            }

            if (method == "DELETE" && segmentuak.Length >= 3)
                return $"Faktura ezabatu da. Faktura ID: {segmentuak[2]}";

            return "Faktura ekintza eginda";
        }

        private static string SortuCrudMezua(string izena, string method, string[] segmentuak)
        {
            return method switch
            {
                "GET" when segmentuak.Length == 2 => $"{izena} zerrenda kontsultatuta",
                "GET" when segmentuak.Length >= 3 => $"{izena.TrimEnd('k')} kontsultatuta. ID: {segmentuak[2]}",
                "POST" => $"{izena.TrimEnd('k')} sortuta",
                "PUT" when segmentuak.Length >= 3 => $"{izena.TrimEnd('k')} eguneratuta. ID: {segmentuak[2]}",
                "DELETE" when segmentuak.Length >= 3 => $"{izena.TrimEnd('k')} ezabatuta. ID: {segmentuak[2]}",
                _ => $"{izena} ekintza eginda"
            };
        }

        private static string SortuErreserbaMezua(string method, string[] segmentuak)
        {
            if (method == "GET" && segmentuak.Length == 2) return "Erreserbak kontsultatuta";
            if (method == "GET" && segmentuak.Length >= 4 && segmentuak[2].Equals("data", StringComparison.OrdinalIgnoreCase))
                return $"Erreserbak kontsultatuta dataren arabera. Data: {segmentuak[3]}";
            if (method == "POST") return "Erreserba sortuta";
            if (method == "PUT" && segmentuak.Length >= 3) return $"Erreserba eguneratuta. Erreserba ID: {segmentuak[2]}";
            if (method == "DELETE" && segmentuak.Length >= 3) return $"Erreserba ezabatu da. Erreserba ID: {segmentuak[2]}";

            return "Erreserba ekintza eginda";
        }

        private static string SortuErreserbaMahaiMezua(string method, string[] segmentuak)
        {
            if (method == "POST") return "Erreserbari mahaiak lotu zaizkio";
            if (method == "GET" && segmentuak.Length >= 4 && segmentuak[2].Equals("erreserba", StringComparison.OrdinalIgnoreCase))
                return $"Erreserbako mahaiak kontsultatuta. Erreserba ID: {segmentuak[3]}";
            if (method == "DELETE" && segmentuak.Length >= 4 && segmentuak[2].Equals("erreserba", StringComparison.OrdinalIgnoreCase))
                return $"Erreserbako mahaiak ezabatu dira. Erreserba ID: {segmentuak[3]}";

            return "Erreserba-mahai ekintza eginda";
        }

        private static string[] LortuPathSegmentuak(PathString path)
        {
            string balioa = path.Value?.Trim('/') ?? string.Empty;
            return string.IsNullOrWhiteSpace(balioa)
                ? Array.Empty<string>()
                : balioa.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        private static string LortuDataTxanda(HttpContext context)
        {
            string? data = context.Request.Query.TryGetValue("data", out var dataQuery) ? dataQuery.ToString() : null;
            string? txanda = context.Request.Query.TryGetValue("txanda", out var txandaQuery) ? txandaQuery.ToString() : null;

            var zatiak = new List<string>();
            if (!string.IsNullOrWhiteSpace(data)) zatiak.Add($"Data: {data}");
            if (!string.IsNullOrWhiteSpace(txanda)) zatiak.Add($"Txanda: {txanda}");

            return string.Join(", ", zatiak);
        }

        private static bool DaEsperoDenNotFound(HttpContext context, int statusCode)
        {
            if (statusCode != 404)
                return false;

            string[] segmentuak = LortuPathSegmentuak(context.Request.Path);

            if (segmentuak.Length >= 5 &&
                segmentuak[0].Equals("api", StringComparison.OrdinalIgnoreCase) &&
                segmentuak[1].Equals("eskaerak", StringComparison.OrdinalIgnoreCase) &&
                segmentuak[2].Equals("mahaia", StringComparison.OrdinalIgnoreCase) &&
                segmentuak[4].Equals("aktiboa", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return segmentuak.Length >= 3 &&
                   segmentuak[0].Equals("api", StringComparison.OrdinalIgnoreCase) &&
                   segmentuak[1].Equals("mahaiak", StringComparison.OrdinalIgnoreCase) &&
                   segmentuak[2].Equals("libre", StringComparison.OrdinalIgnoreCase);
        }

        private static JsonElement? ParseBodyJson(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return null;

            try
            {
                using JsonDocument doc = JsonDocument.Parse(body);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                    return null;

                return doc.RootElement.Clone();
            }
            catch
            {
                return null;
            }
        }

        private static bool TryGetPropertyCaseInsensitive(JsonElement obj, string propertyName, out JsonElement value)
        {
            if (obj.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in obj.EnumerateObject())
                {
                    if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        value = property.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        private static int? LortuInt(JsonElement? json, params string[] propertyNames)
        {
            if (!json.HasValue || json.Value.ValueKind != JsonValueKind.Object)
                return null;

            foreach (string name in propertyNames)
            {
                if (!TryGetPropertyCaseInsensitive(json.Value, name, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int number))
                    return number;

                if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out int parsed))
                    return parsed;
            }

            return null;
        }

        private static int? LortuInt(JsonElement json, params string[] propertyNames)
        {
            if (json.ValueKind != JsonValueKind.Object)
                return null;

            foreach (string name in propertyNames)
            {
                if (!TryGetPropertyCaseInsensitive(json, name, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int number))
                    return number;

                if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out int parsed))
                    return parsed;
            }

            return null;
        }

        private static string? LortuString(JsonElement? json, params string[] propertyNames)
        {
            if (!json.HasValue || json.Value.ValueKind != JsonValueKind.Object)
                return null;

            foreach (string name in propertyNames)
            {
                if (!TryGetPropertyCaseInsensitive(json.Value, name, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.String)
                {
                    string? text = value.GetString();
                    if (!string.IsNullOrWhiteSpace(text))
                        return text;
                }

                if (value.ValueKind == JsonValueKind.Number ||
                    value.ValueKind == JsonValueKind.True ||
                    value.ValueKind == JsonValueKind.False)
                {
                    return value.ToString();
                }
            }

            return null;
        }

        private static string? LortuDecimal(JsonElement? json, params string[] propertyNames)
        {
            if (!json.HasValue || json.Value.ValueKind != JsonValueKind.Object)
                return null;

            foreach (string name in propertyNames)
            {
                if (!TryGetPropertyCaseInsensitive(json.Value, name, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out decimal number))
                    return number.ToString("0.##");

                if (value.ValueKind == JsonValueKind.String && decimal.TryParse(value.GetString(), out decimal parsed))
                    return parsed.ToString("0.##");
            }

            return null;
        }

        private static string? LortuProduktuenLaburpena(JsonElement? json)
        {
            if (!json.HasValue || json.Value.ValueKind != JsonValueKind.Object)
                return null;

            if (!TryGetPropertyCaseInsensitive(json.Value, "Produktuak", out var produktuak) ||
                produktuak.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var laburpena = new List<string>();
            foreach (var produktua in produktuak.EnumerateArray())
            {
                if (produktua.ValueKind != JsonValueKind.Object)
                    continue;

                int? produktuaId = LortuInt(produktua, "ProduktuaId", "produktuaId");
                int? kantitatea = LortuInt(produktua, "Kantitatea", "kantitatea");

                if (produktuaId.HasValue && kantitatea.HasValue)
                {
                    laburpena.Add($"{produktuaId.Value}x{kantitatea.Value}");
                }
                else if (produktuaId.HasValue)
                {
                    laburpena.Add(produktuaId.Value.ToString());
                }
            }

            return laburpena.Count == 0 ? null : string.Join(", ", laburpena);
        }

        private static string? LortuErabiltzaileaBodytik(string body)
        {
            try
            {
                JsonElement? root = ParseBodyJson(body);
                if (!root.HasValue)
                    return null;

                string? erabiltzailea = LortuString(root, "erabiltzailea", "username");
                if (!string.IsNullOrWhiteSpace(erabiltzailea))
                    return erabiltzailea;

                int? erabiltzaileId = LortuInt(root, "erabiltzaileId", "langileaId", "ErabiltzaileId");
                if (erabiltzaileId.HasValue)
                    return $"id:{erabiltzaileId.Value}";
            }
            catch
            {
                // Ezin bada irakurri, ez dugu errorea altxatuko
            }

            return null;
        }
    }
}
