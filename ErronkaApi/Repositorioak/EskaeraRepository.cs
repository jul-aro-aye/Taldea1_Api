using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate.Linq;
using NHSession = NHibernate.ISession;
using NHFactory = NHibernate.ISessionFactory;
using NHibernate;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ErronkaApi.Repositorioak
{
    public class EskaeraRepository
    {
        private readonly NHFactory _sessionFactory;

        public EskaeraRepository(NHFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public EskaeraRepository()
        {
            
        }

        private Eskaera? GetEskaera(NHSession session, int id)
            => session.Get<Eskaera>(id);

        private Produktua? GetProduktua(NHSession session, int id)
            => session.Get<Produktua>(id, LockMode.Upgrade);

        private Mahaia? GetMahaia(NHSession session, int id)
            => session.Get<Mahaia>(id);

        private static string NormalizeTxanda(string? txanda)
        {
            if (string.Equals(txanda, "Afaria", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(txanda, "afaria", StringComparison.OrdinalIgnoreCase))
                return "afaria";

            return "bazkaria";
        }

        private static string FormatTxanda(string? txanda)
        {
            return string.Equals(txanda, "afaria", StringComparison.OrdinalIgnoreCase)
                ? "Afaria"
                : "Bazkaria";
        }

        private static DateTime SortuEskaerarenData(DateTime? data, string? txanda)
        {
            if (!data.HasValue)
                return DateTime.Now;

            var ordua = NormalizeTxanda(txanda) == "afaria" ? 20 : 13;
            return data.Value.Date.AddHours(ordua);
        }

        private static string InferituTxanda(DateTime data)
        {
            return data.Hour >= 18 ? "afaria" : "bazkaria";
        }

        private static bool EskaeraBlokeatutaDago(Eskaera eskaera)
        {
            return string.Equals(eskaera.egoera, "ordainketa_pendiente", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(eskaera.egoera, "itxita", StringComparison.OrdinalIgnoreCase);
        }

        private static string FormateatuErrorea(Exception ex)
        {
            var mezuak = new List<string>();
            var unekoa = ex;

            while (unekoa != null)
            {
                if (!string.IsNullOrWhiteSpace(unekoa.Message) && !mezuak.Contains(unekoa.Message))
                    mezuak.Add(unekoa.Message);

                unekoa = unekoa.InnerException;
            }

            return string.Join(" | ", mezuak);
        }

        private string LortuEskaerarenTxanda(NHSession session, Eskaera eskaera)
        {
            ZiurtatuEskaerenTxandaZutabea(session);

            if (!string.IsNullOrWhiteSpace(eskaera.txanda))
                return NormalizeTxanda(eskaera.txanda);

            if (eskaera.erreserbaId.HasValue)
            {
                var erreserba = session.Get<Erreserba>(eskaera.erreserbaId.Value);
                if (erreserba != null)
                    return NormalizeTxanda(erreserba.txanda);
            }

            return InferituTxanda(eskaera.sortzeData);
        }

        private bool EskaeraDagokioDataTxandari(
            NHSession session,
            Eskaera eskaera,
            DateTime data,
            string txanda)
        {
            var dataHelburua = data.Date;
            var txandaNormalizatua = NormalizeTxanda(txanda);

            if (eskaera.erreserbaId.HasValue)
            {
                var erreserba = session.Get<Erreserba>(eskaera.erreserbaId.Value);
                if (erreserba != null)
                {
                    return erreserba.erreserbaData.Date == dataHelburua &&
                           NormalizeTxanda(erreserba.txanda) == txandaNormalizatua;
                }
            }

            return eskaera.sortzeData.Date == dataHelburua &&
                   InferituTxanda(eskaera.sortzeData) == txandaNormalizatua;
        }

        private Eskaera? GetEskaeraAktiboaMahaiarentzat(
            NHSession session,
            int mahaiaId,
            DateTime? data = null,
            string? txanda = null)
        {
            ZiurtatuEskaerenTxandaZutabea(session);

            var eskaerak = session.Query<Eskaera>()
                .Where(e =>
                    e.mahaia_id == mahaiaId &&
                    (e.egoera == "irekita" || e.egoera == "ordainketa_pendiente"))
                .OrderByDescending(e => e.sortzeData)
                .ToList();

            if (!data.HasValue)
                return eskaerak.FirstOrDefault();

            var dataErabilgarria = data.Value.Date;
            var txandaErabilgarria = NormalizeTxanda(txanda);

            return eskaerak.FirstOrDefault(e =>
                EskaeraDagokioDataTxandari(session, e, dataErabilgarria, txandaErabilgarria));
        }

        private sealed class OsagaiBeharra
        {
            public int OsagaiaId { get; set; }
            public string Izena { get; set; } = string.Empty;
            public decimal BeharrezkoKantitatea { get; set; }
            public decimal StockAktuala { get; set; }
            public string Unitatea { get; set; } = string.Empty;
        }

        private sealed class ProduktuEskaria
        {
            public int ProduktuaId { get; set; }
            public int Kantitatea { get; set; }
        }

        private sealed class DeskontuDatuak
        {
            public string? Kodea { get; set; }
            public decimal Portzentaia { get; set; }
        }

        private static decimal BiribilduDirua(decimal balioa)
        {
            return Math.Round(balioa, 2, MidpointRounding.AwayFromZero);
        }

        private static decimal NormalizatuDeskontuPortzentaia(decimal? deskontuPortzentaia)
        {
            if (!deskontuPortzentaia.HasValue)
                return 0;

            var balioa = deskontuPortzentaia.Value;
            if (balioa < 0)
                return 0;
            if (balioa > 100)
                return 100;

            return BiribilduDirua(balioa);
        }

        private static string? GarbituDeskontuKodea(string? deskontuKodea)
        {
            var kodea = deskontuKodea?.Trim();
            return string.IsNullOrWhiteSpace(kodea) ? null : kodea;
        }

        private static bool BadagoGutxienezProduktuBat(IEnumerable<ProduktuEskaria>? produktuEskariak)
        {
            return produktuEskariak != null && produktuEskariak.Any(p => p.ProduktuaId > 0 && p.Kantitatea > 0);
        }

        private static bool SukaldeaEgindaDago(string? egoera)
        {
            return string.Equals(egoera, "eginda", StringComparison.OrdinalIgnoreCase);
        }

        private static string LortuOdooApiBaseUrl()
        {
            var inguruneBalioa = Environment.GetEnvironmentVariable("ODOO_API_BASE_URL");
            if (!string.IsNullOrWhiteSpace(inguruneBalioa))
                return inguruneBalioa.Trim().TrimEnd('/') + "/";

            try
            {
                var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                if (File.Exists(appSettingsPath))
                {
                    var json = JObject.Parse(File.ReadAllText(appSettingsPath));
                    var balioa = json["OdooApiBaseUrl"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(balioa))
                        return balioa.Trim().TrimEnd('/') + "/";
                }
            }
            catch
            {
            }

            return "http://localhost:5015/";
        }

        private void ZiurtatuEskaerenTxandaZutabea(NHSession session)
        {
            var txandaColumnCount = Convert.ToInt32(session.CreateSQLQuery(@"
                SELECT COUNT(*)
                FROM information_schema.columns
                WHERE table_schema = DATABASE()
                  AND table_name = 'eskaerak'
                  AND column_name = 'txanda'")
                .UniqueResult());

            if (txandaColumnCount == 0)
            {
                session.CreateSQLQuery(@"
                    ALTER TABLE eskaerak
                    ADD COLUMN txanda VARCHAR(20) NULL AFTER sortze_data")
                    .ExecuteUpdate();
            }

            session.CreateSQLQuery(@"
                UPDATE eskaerak e
                LEFT JOIN erreserbak r ON r.id = e.erreserba_id
                SET e.txanda = CASE
                    WHEN r.txanda IS NOT NULL AND TRIM(r.txanda) <> '' THEN
                        CASE
                            WHEN LOWER(TRIM(r.txanda)) = 'afaria' THEN 'afaria'
                            ELSE 'bazkaria'
                        END
                    WHEN HOUR(e.sortze_data) >= 18 THEN 'afaria'
                    ELSE 'bazkaria'
                END
                WHERE e.txanda IS NULL OR TRIM(e.txanda) = ''")
                .ExecuteUpdate();
        }

        private (bool success, string? error, decimal portzentaia) BalioztatuDeskontuKodea(string kodea)
        {
            try
            {
                using var client = new HttpClient
                {
                    BaseAddress = new Uri(LortuOdooApiBaseUrl())
                };

                var body = JsonConvert.SerializeObject(new { kodea });
                using var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = client.PostAsync("api/deskontuak/egiaztatu", content).Result;

                if (!response.IsSuccessStatusCode)
                    return (false, "Ezin izan da deskontu kodea egiaztatu", 0);

                var json = response.Content.ReadAsStringAsync().Result;
                var emaitza = JsonConvert.DeserializeObject<OdooDeskontuEmaitzaBarne>(json);

                if (emaitza == null || !emaitza.ExistitzenDa || emaitza.Balioa <= 0)
                    return (false, "Kodea ez da zuzena edo ez dago aktibo.", 0);

                return (true, null, NormalizatuDeskontuPortzentaia(Convert.ToDecimal(emaitza.Balioa)));
            }
            catch
            {
                return (false, "Ezin izan da deskontu kodea egiaztatu", 0);
            }
        }

        private void ZiurtatuDeskontuenTaula(NHSession session)
        {
            const string sql = @"
                CREATE TABLE IF NOT EXISTS eskaera_deskontuak (
                    id INT NOT NULL AUTO_INCREMENT,
                    eskaera_id INT NOT NULL,
                    kodea VARCHAR(120) NOT NULL,
                    deskontu_portzentaia DECIMAL(10,2) NOT NULL,
                    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (id),
                    UNIQUE KEY uq_eskaera_deskontuak_eskaera_id (eskaera_id),
                    CONSTRAINT fk_eskaera_deskontuak_eskaera
                        FOREIGN KEY (eskaera_id) REFERENCES eskaerak(id)
                        ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";

            session.CreateSQLQuery(sql).ExecuteUpdate();
        }

        private DeskontuDatuak LortuEskaerarenDeskontua(NHSession session, int eskaeraId)
        {
            ZiurtatuDeskontuenTaula(session);

            const string sql = @"
                SELECT kodea, deskontu_portzentaia
                FROM eskaera_deskontuak
                WHERE eskaera_id = :eskaeraId
                LIMIT 1";

            var lerroa = session.CreateSQLQuery(sql)
                .SetParameter("eskaeraId", eskaeraId)
                .List<object[]>()
                .FirstOrDefault();

            if (lerroa == null)
                return new DeskontuDatuak();

            return new DeskontuDatuak
            {
                Kodea = Convert.ToString(lerroa[0]),
                Portzentaia = NormalizatuDeskontuPortzentaia(Convert.ToDecimal(lerroa[1]))
            };
        }

        private void GordeEskaerarenDeskontua(
            NHSession session,
            int eskaeraId,
            string? deskontuKodea,
            decimal deskontuPortzentaia)
        {
            ZiurtatuDeskontuenTaula(session);

            var kodeaGarbitua = GarbituDeskontuKodea(deskontuKodea);
            var portzentaiaNormalizatua = NormalizatuDeskontuPortzentaia(deskontuPortzentaia);

            if (kodeaGarbitua == null || portzentaiaNormalizatua <= 0)
            {
                const string ezabatuSql = @"
                    DELETE FROM eskaera_deskontuak
                    WHERE eskaera_id = :eskaeraId";

                session.CreateSQLQuery(ezabatuSql)
                    .SetParameter("eskaeraId", eskaeraId)
                    .ExecuteUpdate();
                return;
            }

            const string txertatuSql = @"
                INSERT INTO eskaera_deskontuak (eskaera_id, kodea, deskontu_portzentaia, sortze_data)
                VALUES (:eskaeraId, :kodea, :deskontuPortzentaia, NOW())
                ON DUPLICATE KEY UPDATE
                    kodea = VALUES(kodea),
                    deskontu_portzentaia = VALUES(deskontu_portzentaia),
                    sortze_data = NOW()";

            session.CreateSQLQuery(txertatuSql)
                .SetParameter("eskaeraId", eskaeraId)
                .SetParameter("kodea", kodeaGarbitua)
                .SetParameter("deskontuPortzentaia", portzentaiaNormalizatua)
                .ExecuteUpdate();
        }

        private decimal KalkulatuDeskontuarekin(decimal azpitotala, decimal deskontuPortzentaia)
        {
            var portzentaiaNormalizatua = NormalizatuDeskontuPortzentaia(deskontuPortzentaia);
            if (portzentaiaNormalizatua <= 0)
                return BiribilduDirua(azpitotala);

            var deskontuKopurua = BiribilduDirua(azpitotala * (portzentaiaNormalizatua / 100m));
            return BiribilduDirua(Math.Max(0, azpitotala - deskontuKopurua));
        }

        private List<OsagaiBeharra> LortuOsagaiBeharrak(NHSession session, int produktuaId, int produktuKopurua)
        {
            const string sql = @"
                SELECT
                    o.id AS osagaia_id,
                    o.izena AS osagaia_izena,
                    COALESCE(po.kantitatea, 0) AS kantitatea,
                    COALESCE(o.stock_aktuala, 0) AS stock_aktuala,
                    COALESCE(po.unitatea, '') AS unitatea
                FROM produktu_osagaiak po
                INNER JOIN osagaiak o ON o.id = po.osagaia_id
                WHERE po.produktua_id = :produktuaId
                FOR UPDATE";

            var rows = session.CreateSQLQuery(sql)
                .SetParameter("produktuaId", produktuaId)
                .List<object[]>();

            return rows
                .Select(row => new OsagaiBeharra
                {
                    OsagaiaId = Convert.ToInt32(row[0]),
                    Izena = Convert.ToString(row[1]) ?? string.Empty,
                    BeharrezkoKantitatea = Convert.ToDecimal(row[2]) * produktuKopurua,
                    StockAktuala = Convert.ToDecimal(row[3]),
                    Unitatea = Convert.ToString(row[4]) ?? string.Empty
                })
                .ToList();
        }

        private void EguneratuOsagaiStocka(NHSession session, int osagaiaId, decimal stockBerria)
        {
            const string sql = @"
                UPDATE osagaiak
                SET stock_aktuala = :stockBerria
                WHERE id = :osagaiaId";

            session.CreateSQLQuery(sql)
                .SetParameter("stockBerria", stockBerria)
                .SetParameter("osagaiaId", osagaiaId)
                .ExecuteUpdate();
        }

        private List<ProduktuEskaria> NormalizatuProduktuEskariak(IEnumerable<ProduktuEskaria> produktuEskariak)
        {
            return produktuEskariak
                .Where(p => p.Kantitatea > 0)
                .GroupBy(p => p.ProduktuaId)
                .Select(g => new ProduktuEskaria
                {
                    ProduktuaId = g.Key,
                    Kantitatea = g.Sum(x => x.Kantitatea)
                })
                .ToList();
        }

        private (List<string> faltan, List<ProduktuEskaria> produktuak, Dictionary<int, OsagaiBeharra> osagaiak)
            BalidatuEskariaStockarekin(NHSession session, IEnumerable<ProduktuEskaria> produktuEskariak)
        {
            var produktuak = NormalizatuProduktuEskariak(produktuEskariak);
            var faltan = new List<string>();
            var osagaienEskariak = new Dictionary<int, OsagaiBeharra>();

            foreach (var produktuEskaria in produktuak)
            {
                var produktua = GetProduktua(session, produktuEskaria.ProduktuaId);
                if (produktua == null)
                {
                    faltan.Add($"Produktua {produktuEskaria.ProduktuaId}");
                    continue;
                }

                if (produktua.stock_aktuala < produktuEskaria.Kantitatea)
                    faltan.Add(produktua.izena);

                foreach (var osagaiBeharra in LortuOsagaiBeharrak(session, produktuEskaria.ProduktuaId, produktuEskaria.Kantitatea))
                {
                    if (osagaienEskariak.TryGetValue(osagaiBeharra.OsagaiaId, out var existitzenDena))
                    {
                        existitzenDena.BeharrezkoKantitatea += osagaiBeharra.BeharrezkoKantitatea;
                        continue;
                    }

                    osagaienEskariak[osagaiBeharra.OsagaiaId] = osagaiBeharra;
                }
            }

            foreach (var osagaiEskaria in osagaienEskariak.Values)
            {
                if (osagaiEskaria.StockAktuala < osagaiEskaria.BeharrezkoKantitatea)
                    faltan.Add($"{osagaiEskaria.Izena} ({osagaiEskaria.BeharrezkoKantitatea:0.##} {osagaiEskaria.Unitatea})");
            }

            return (faltan, produktuak, osagaienEskariak);
        }

        private void AplikatuEskaeraProduktuak(
            NHSession session,
            Eskaera eskaera,
            List<ProduktuEskaria> produktuEskariak,
            Dictionary<int, OsagaiBeharra> osagaienEskariak)
        {
            foreach (var produktuEskaria in produktuEskariak)
            {
                var produktua = GetProduktua(session, produktuEskaria.ProduktuaId);
                if (produktua == null)
                    continue;

                produktua.stock_aktuala -= produktuEskaria.Kantitatea;
                session.Update(produktua);

                var eskaeraProduktua = new EskaeraProduktuak
                {
                    Eskaera = eskaera,
                    Produktua = produktua,
                    Kantitatea = produktuEskaria.Kantitatea,
                    Egoera = "bidalita",
                    PrezioUnitarioa = produktua.prezioa,
                    Guztira = produktua.prezioa * produktuEskaria.Kantitatea
                };

                session.Save(eskaeraProduktua);
                eskaera.EskaeraProduktuak.Add(eskaeraProduktua);
            }

            foreach (var osagaiEskaria in osagaienEskariak.Values)
                EguneratuOsagaiStocka(
                    session,
                    osagaiEskaria.OsagaiaId,
                    osagaiEskaria.StockAktuala - osagaiEskaria.BeharrezkoKantitatea);
        }

        private void LeheneratuEskaeraProduktuak(NHSession session, Eskaera eskaera, bool ezabatuLerroak)
        {
            foreach (var ep in eskaera.EskaeraProduktuak.ToList())
            {
                var produktua = GetProduktua(session, ep.Produktua.id);
                if (produktua != null)
                {
                    produktua.stock_aktuala += ep.Kantitatea;
                    session.Update(produktua);
                }

                foreach (var osagaiBeharra in LortuOsagaiBeharrak(session, ep.Produktua.id, ep.Kantitatea))
                    EguneratuOsagaiStocka(
                        session,
                        osagaiBeharra.OsagaiaId,
                        osagaiBeharra.StockAktuala + osagaiBeharra.BeharrezkoKantitatea);

                if (ezabatuLerroak)
                {
                    eskaera.EskaeraProduktuak.Remove(ep);
                    session.Delete(ep);
                }
            }
        }

        private decimal KalkulatuEskaeraGuztira(NHSession session, int eskaeraId)
        {
            return session.Query<EskaeraProduktuak>()
                .Where(ep => ep.Eskaera.id == eskaeraId)
                .ToList()
                .Sum(ep => ep.Guztira > 0 ? ep.Guztira : ep.PrezioUnitarioa * ep.Kantitatea);
        }

        private void EguneratuFakturaTotala(NHSession session, int eskaeraId)
        {
            EguneratuFakturaTotala(session, eskaeraId, KalkulatuEskaeraGuztira(session, eskaeraId));
        }

        private void EguneratuFakturaTotala(NHSession session, int eskaeraId, decimal guztira)
        {
            var fakturak = session.Query<Faktura>()
                .Where(f => f.eskaeraId == eskaeraId)
                .OrderBy(f => f.id)
                .ToList();
            var faktura = fakturak.FirstOrDefault();

            if (faktura == null)
            {
                faktura = new Faktura
                {
                    eskaeraId = eskaeraId,
                    pdfIzena = string.Empty,
                    data = DateTime.Now,
                    guztira = BiribilduDirua(guztira)
                };

                session.Save(faktura);
                return;
            }

            foreach (var soberan in fakturak.Skip(1))
                session.Delete(soberan);

            faktura.data = DateTime.Now;
            faktura.guztira = BiribilduDirua(guztira);
            session.SaveOrUpdate(faktura);
        }

        private EskaeraDTO MapToEskaeraDTO(NHSession session, Eskaera e)
        {
            var deskontua = LortuEskaerarenDeskontua(session, e.id);
            return new EskaeraDTO
            {
                Id = e.id,
                Izena = $"Eskaera #{e.id}",
                MahaiaId = e.mahaia_id ?? 0,
                Komensalak = e.komensalak,
                Egoera = e.egoera,
                Data = e.sortzeData.ToString("yyyy-MM-dd HH:mm"),
                Txanda = FormatTxanda(LortuEskaerarenTxanda(session, e)),
                SukaldeaEgoera = e.sukaldeaEgoera,
                DeskontuKodea = GarbituDeskontuKodea(deskontua.Kodea),
                DeskontuPortzentaia = NormalizatuDeskontuPortzentaia(deskontua.Portzentaia)
            };
        }

        public virtual EskaeraLortuDTO MapToEskaeraLortuDTO(EskaeraProduktuak ep)
        {
            return new EskaeraLortuDTO
            {
                ProduktuaId = ep.Produktua.id,
                ProduktuaIzena = ep.Produktua.izena,
                PrezioUnitarioa = ep.PrezioUnitarioa,
                Kantitatea = 1
            };
        }

        public virtual (bool success, string? error, int? data, List<string>? details)
            SortuEskaera(EskaeraSortuDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                ZiurtatuEskaerenTxandaZutabea(session);

                var mahaia = GetMahaia(session, dto.MahaiaId);
                if (mahaia == null)
                    return (false, "Mahaia ez da aurkitu", null, null);

                if (dto.Komensalak <= 0)
                    return (false, "Komensalak 1 edo handiagoa izan behar dira", null, null);

                if (mahaia.kapazitatea > 0 && dto.Komensalak > mahaia.kapazitatea)
                    return (false, "Komensalak mahaiko kapazitatea baino handiagoak dira", null, null);

                var produktuEskariGordinak = dto.Produktuak?
                    .Select(p => new ProduktuEskaria
                    {
                        ProduktuaId = p.ProduktuaId,
                        Kantitatea = p.Kantitatea
                    })
                    .ToList() ?? new List<ProduktuEskaria>();

                if (!BadagoGutxienezProduktuBat(produktuEskariGordinak))
                    return (false, "Eskaerak gutxienez produktu bat behar du", null, null);

                if (dto.ErreserbaId.HasValue)
                {
                    var erreserba = session.Get<Erreserba>(dto.ErreserbaId.Value);

                    if (erreserba == null)
                        return (false, "Erreserba ez da aurkitu", null, null);

                    if (erreserba.mahaiaId != dto.MahaiaId)
                        return (false, "Erreserba ez dator bat aukeratutako mahaiarekin", null, null);
                }

                var eskaeraAktiboa = GetEskaeraAktiboaMahaiarentzat(
                    session,
                    dto.MahaiaId,
                    dto.Data,
                    dto.Txanda);

                var (faltan, produktuEskariak, osagaiEskariak) = BalidatuEskariaStockarekin(
                    session,
                    produktuEskariGordinak);

                if (faltan.Any())
                    return (false, "Stock gabe dauden produktu edo osagaiak daude", null, faltan);

                mahaia.egoera = "okupatuta";
                session.Update(mahaia);

                var eskaeraBerria = eskaeraAktiboa == null;

                if (!eskaeraBerria && EskaeraBlokeatutaDago(eskaeraAktiboa!))
                    return (false, "Eskaera ordainketa pendiente edo itxita dago; ezin da gehiago aldatu", null, null);

                var eskaera = eskaeraAktiboa ?? new Eskaera
                {
                    erabiltzaileId = dto.ErabiltzaileId,
                    komensalak = dto.Komensalak,
                    egoera = "irekita",
                    sukaldeaEgoera = "bidalita",
                    sortzeData = SortuEskaerarenData(dto.Data, dto.Txanda),
                    txanda = NormalizeTxanda(dto.Txanda),
                    mahaia_id = dto.MahaiaId,
                    erreserbaId = dto.ErreserbaId
                };

                if (eskaeraBerria)
                {
                    session.Save(eskaera);

                    session.Save(new EskaeraMahaiak
                    {
                        Eskaera = eskaera,
                        Mahaia = mahaia
                    });
                }
                else
                {
                    eskaera.erabiltzaileId = dto.ErabiltzaileId;
                    eskaera.komensalak = Math.Max(eskaera.komensalak, dto.Komensalak);
                    eskaera.egoera = "irekita";
                    eskaera.sukaldeaEgoera = "bidalita";
                    eskaera.txanda = NormalizeTxanda(dto.Txanda);

                    if (!eskaera.erreserbaId.HasValue && dto.ErreserbaId.HasValue)
                        eskaera.erreserbaId = dto.ErreserbaId;

                    session.Update(eskaera);
                }

                AplikatuEskaeraProduktuak(session, eskaera, produktuEskariak, osagaiEskariak);

                EguneratuFakturaTotala(session, eskaera.id);

                tx.Commit();
                return (true, null, eskaera.id, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, FormateatuErrorea(ex), null, null);
            }
        }

        public virtual (bool success, string? error, List<EskaeraDTO>? data)
            LortuEskaerak()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                ZiurtatuEskaerenTxandaZutabea(session);

                var lista = session.Query<Eskaera>()
                    .Where(e => e.egoera == "irekita")
                    .OrderByDescending(e => e.sortzeData)
                    .ToList()
                    .Select(e => MapToEskaeraDTO(session, e))
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error, EskaeraDTO? data)
            LortuEskaeraAktiboaMahaika(int mahaiaId, DateTime? data = null, string? txanda = null)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                ZiurtatuEskaerenTxandaZutabea(session);
                var eskaera = GetEskaeraAktiboaMahaiarentzat(session, mahaiaId, data, txanda);

                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu", null);

                return (true, null, MapToEskaeraDTO(session, eskaera));
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error, List<EskaeraLortuDTO>? data)
            LortuEskaeraProduktuak(int eskaeraId)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var produktuak = session.Query<EskaeraProduktuak>()
                    .Where(ep => ep.Eskaera.id == eskaeraId)
                    .ToList();

                if (!produktuak.Any())
                    return (false, "Eskaera ez da aurkitu", null);

                var lista = new List<EskaeraLortuDTO>();

                foreach (var ep in produktuak)
                    for (int i = 0; i < ep.Kantitatea; i++)
                        lista.Add(MapToEskaeraLortuDTO(ep));

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error) EzabatuEskaera(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                ZiurtatuEskaerenTxandaZutabea(session);
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu");

                if (EskaeraBlokeatutaDago(eskaera))
                    return (false, "Eskaera ordainketa pendiente edo itxita dago; ezin da ezabatu");

                LeheneratuEskaeraProduktuak(session, eskaera, true);

                var fakturak = session.Query<Faktura>()
                    .Where(f => f.eskaeraId == eskaeraId)
                    .ToList();

                foreach (var faktura in fakturak)
                    session.Delete(faktura);

                if (eskaera.mahaia_id.HasValue)
                {
                    var mahaia = GetMahaia(session, eskaera.mahaia_id.Value);
                    if (mahaia != null)
                    {
                        mahaia.egoera = "libre";
                        session.Update(mahaia);
                    }
                }

                session.Delete(eskaera);
                tx.Commit();
                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual (bool success, string? error, int? data)
            LortuMahaiKapazitatea(int mahaiaId)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var mahaia = GetMahaia(session, mahaiaId);

                if (mahaia == null)
                    return (false, "Mahaia ez da aurkitu", null);

                    return (true, null, mahaia.kapazitatea);
                }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error, List<string>? details)
            EguneratuEskaera(int eskaeraId, EskaeraEguneratuDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                ZiurtatuEskaerenTxandaZutabea(session);
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu", null);

                if (EskaeraBlokeatutaDago(eskaera))
                    return (false, "Eskaera ordainketa pendiente edo itxita dago; ezin da gehiago aldatu", null);

                if (dto.Komensalak <= 0)
                    return (false, "Komensalak 1 edo handiagoa izan behar dira", null);

                var produktuEskariak = dto.Produktuak?
                    .Select(p => new ProduktuEskaria
                    {
                        ProduktuaId = p.ProduktuaId,
                        Kantitatea = p.Kantitatea
                    })
                    .ToList() ?? new List<ProduktuEskaria>();

                if (!produktuEskariak.Any(p => p.Kantitatea > 0))
                    return (false, "Eskaerak gutxienez produktu bat behar du", null);

                LeheneratuEskaeraProduktuak(session, eskaera, true);

                var (faltan, produktuNormalizatuak, osagaiEskariak) =
                    BalidatuEskariaStockarekin(session, produktuEskariak);

                if (faltan.Any())
                    return (false, "Stock gabe dauden produktu edo osagaiak daude", faltan);

                eskaera.komensalak = dto.Komensalak;
                eskaera.egoera = "irekita";
                eskaera.sukaldeaEgoera = "bidalita";
                session.Update(eskaera);

                AplikatuEskaeraProduktuak(session, eskaera, produktuNormalizatuak, osagaiEskariak);
                EguneratuFakturaTotala(session, eskaera.id);

                tx.Commit();
                return (true, null, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, FormateatuErrorea(ex), null);
            }
        }

        public virtual (bool success, string? error)
            EguneratuSukaldeaEgoera(int eskaeraId, string egoera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                ZiurtatuEskaerenTxandaZutabea(session);
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu");

                eskaera.sukaldeaEgoera = egoera;
                session.Update(eskaera);

                tx.Commit();
                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual (bool success, string? error)
            OrdaintzeraBidali(int eskaeraId)
        {
            return OrdaintzeraBidali(eskaeraId, null);
        }

        public virtual (bool success, string? error)
            OrdaintzeraBidali(int eskaeraId, EskaeraOrdainduDTO? dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                ZiurtatuEskaerenTxandaZutabea(session);
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu");

                if (EskaeraBlokeatutaDago(eskaera))
                    return (false, "Eskaera ordainketa pendiente edo itxita dago; ezin da gehiago aldatu");

                var deskontuPortzentaia = NormalizatuDeskontuPortzentaia(dto?.DeskontuPortzentaia);
                var deskontuKodea = GarbituDeskontuKodea(dto?.DeskontuKodea);

                if (deskontuPortzentaia > 0 && deskontuKodea == null)
                    return (false, "Deskontu kodea behar da deskontua aplikatzeko");

                if (deskontuPortzentaia == 0)
                    deskontuKodea = null;

                if (deskontuKodea != null)
                {
                    var (baliozkoa, errorea, portzentaiaBalioztatua) = BalioztatuDeskontuKodea(deskontuKodea);
                    if (!baliozkoa)
                        return (false, errorea ?? "Kodea ez da zuzena edo ez dago aktibo.");

                    if (deskontuPortzentaia > 0 && deskontuPortzentaia != portzentaiaBalioztatua)
                        return (false, "Deskontuaren balioa ez dator bat kodearekin.");

                    deskontuPortzentaia = portzentaiaBalioztatua;
                }

                var azpitotala = KalkulatuEskaeraGuztira(session, eskaeraId);
                if (azpitotala <= 0)
                    return (false, "Eskaerak gutxienez produktu bat behar du");

                var guztira = deskontuPortzentaia > 0
                    ? KalkulatuDeskontuarekin(azpitotala, deskontuPortzentaia)
                    : BiribilduDirua(azpitotala);

                GordeEskaerarenDeskontua(session, eskaeraId, deskontuKodea, deskontuPortzentaia);
                EguneratuFakturaTotala(session, eskaeraId, guztira);

                eskaera.egoera = "ordainketa_pendiente";
                session.Update(eskaera);

                tx.Commit();
                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual (bool success, string? error, string? path)
            SortuFaktura(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                ZiurtatuEskaerenTxandaZutabea(session);
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu", null);

                if (!SukaldeaEgindaDago(eskaera.sukaldeaEgoera))
                    return (false, "Ezin da tiketa sortu sukaldeko egoera 'eginda' izan arte.", null);

                var produktuak = session.Query<EskaeraProduktuak>()
                    .Where(p => p.Eskaera.id == eskaeraId)
                    .ToList();

                if (!produktuak.Any())
                    return (false, "Eskaerak gutxienez produktu bat behar du", null);

                var deskontua = LortuEskaerarenDeskontua(session, eskaeraId);
                var deskontuPortzentaia = NormalizatuDeskontuPortzentaia(deskontua.Portzentaia);
                var deskontuKodea = GarbituDeskontuKodea(deskontua.Kodea);
                var badagoDeskontua = deskontuPortzentaia > 0 && deskontuKodea != null;

                decimal azpitotala = 0;

                string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string carpetaFakturak = Path.Combine(escritorio, "fakturak");

                if (!Directory.Exists(carpetaFakturak))
                    Directory.CreateDirectory(carpetaFakturak);

                string filename = Path.Combine(carpetaFakturak, $"Faktura_Eskaera_{eskaeraId}.pdf");

                using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    float mmToPoints = 2.83465f;
                    var pageWidth = 80 * mmToPoints;
                    var pageHeight = 1000f;

                    var doc = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(pageWidth, pageHeight));
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

                    doc.Add(new iTextSharp.text.Paragraph("Beasain Jatetxea", titleFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingAfter = 3f });
                    doc.Add(new iTextSharp.text.Paragraph("NIF: X12345678", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER });
                    doc.Add(new iTextSharp.text.Paragraph($"Faktura #: {eskaeraId}", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingAfter = 5f });
                    doc.Add(new iTextSharp.text.Paragraph($"Mahaia: {eskaera.mahaia_id}   Data: {eskaera.sortzeData:dd/MM/yyyy HH:mm}", normalFont) { SpacingAfter = 5f });

                    foreach (var p in produktuak)
                    {
                        string produktuIzena = p.Produktua?.izena ?? "Ezezaguna";
                        decimal prezioa = BiribilduDirua(p.PrezioUnitarioa);
                        int kantitatea = p.Kantitatea;
                        decimal lineaTotala = BiribilduDirua(prezioa * kantitatea);
                        azpitotala += lineaTotala;
                        var prezioAplikatua = badagoDeskontua
                            ? BiribilduDirua(prezioa * (1 - (deskontuPortzentaia / 100m)))
                            : prezioa;
                        var lineaTotalaAplikatua = BiribilduDirua(prezioAplikatua * kantitatea);

                        doc.Add(new iTextSharp.text.Paragraph(produktuIzena, normalFont) { SpacingAfter = 1f });
                        doc.Add(new iTextSharp.text.Paragraph($"{kantitatea} x {prezioAplikatua:C}    {lineaTotalaAplikatua:C}", normalFont) { SpacingAfter = 3f });
                    }

                    azpitotala = BiribilduDirua(azpitotala);
                    var total = badagoDeskontua
                        ? KalkulatuDeskontuarekin(azpitotala, deskontuPortzentaia)
                        : azpitotala;
                    var deskontuKopurua = BiribilduDirua(Math.Max(0, azpitotala - total));

                    doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));
                    if (badagoDeskontua)
                    {
                        doc.Add(new iTextSharp.text.Paragraph($"AZPITOTALA: {azpitotala:C}", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 2f });
                        doc.Add(new iTextSharp.text.Paragraph($"DESKONTUA ({deskontuKodea} - {deskontuPortzentaia:0.##}%): -{deskontuKopurua:C}", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 2f });
                    }
                    doc.Add(new iTextSharp.text.Paragraph($"TOTALA: {total:C}", titleFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 5f });
                    doc.Add(new iTextSharp.text.Paragraph("Prezioak BEZ barne daude", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 2f });

                    doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph("Enpresaren datuak: NIF: X12345678 | PV: 001", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingBefore = 8f });
                    doc.Add(new iTextSharp.text.Paragraph("ESKERRIK ASKO", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingBefore = 8f });

                    doc.Close();
                    var faktura = session.Query<Faktura>()
                        .FirstOrDefault(f => f.eskaeraId == eskaeraId);

                    if (faktura == null)
                    {
                        faktura = new Faktura
                        {
                            eskaeraId = eskaeraId
                        };
                    }

                    faktura.pdfIzena = filename;
                    faktura.data = DateTime.Now;
                    faktura.guztira = total;
                    session.SaveOrUpdate(faktura);
                }

                eskaera.egoera = "itxita";
                eskaera.itxieraData = DateTime.Now;
                session.Update(eskaera);

                if (eskaera.erreserbaId.HasValue)
                {
                    var erreserba = session.Get<Erreserba>(eskaera.erreserbaId.Value);
                    if (erreserba != null)
                    {
                        session.CreateSQLQuery(@"
                            UPDATE erreserbak
                            SET egoera = :egoera
                            WHERE id = :id")
                            .SetParameter("egoera", "eginda")
                            .SetParameter("id", erreserba.id)
                            .ExecuteUpdate();
                    }
                }

                if (eskaera.mahaia_id.HasValue)
                {
                    var mahaia = GetMahaia(session, eskaera.mahaia_id.Value);
                    if (mahaia != null)
                    {
                        mahaia.egoera = "libre";
                        session.Update(mahaia);
                    }
                }

                tx.Commit();
                return (true, null, filename);
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }
                return (false, "Arazoa faktura sortzean: " + FormateatuErrorea(ex), null);
            }
        }

        public virtual (bool success, string? error, List<EskaeraDTO>? data)
            LortuEskaerakOrdaintzeko()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                ZiurtatuEskaerenTxandaZutabea(session);

                var lista = session.Query<Eskaera>()
                    .Where(e => e.egoera == "ordainketa_pendiente")
                    .ToList()
                    .Select(e => MapToEskaeraDTO(session, e))
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        private sealed class OdooDeskontuEmaitzaBarne
        {
            public bool ExistitzenDa { get; set; }
            public double Balioa { get; set; }
        }
    }
}
