using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;

namespace ErronkaApi.Repositorioak
{
    public class OdooRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public OdooRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public virtual (bool success, string? error, OdooSyncDTO? data) LortuSinkronizazioDatuak()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var zerbitzariak = session.Query<Erabiltzailea>()
                    .Where(x => !x.ezabatua)
                    .Where(x => x.rola.id == 2)
                    .OrderBy(x => x.erabiltzailea)
                    .ToList()
                    .Select(MapToZerbitzariaDTO)
                    .ToList();

                var platerak = session.Query<Produktua>()
                    .OrderBy(x => x.izena)
                    .ToList()
                    .Select(MapToProduktuaDTO)
                    .ToList();

                var mahaiak = session.Query<Mahaia>()
                    .OrderBy(x => x.zenbakia)
                    .ToList()
                    .Select(MapToMahaiaDTO)
                    .ToList();

                var fakturak = session.Query<Faktura>()
                    .OrderByDescending(x => x.data)
                    .ThenByDescending(x => x.id)
                    .ToList()
                    .Select(MapToFakturaDTO)
                    .ToList();

                var erabiltzaileak = session.Query<Erabiltzailea>()
                    .Where(x => !x.ezabatua)
                    .ToList()
                    .ToDictionary(x => x.id, x => x.erabiltzailea);

                var eskaerak = session.Query<Eskaera>()
                    .FetchMany(x => x.EskaeraProduktuak)
                    .ThenFetch(x => x.Produktua)
                    .OrderByDescending(x => x.sortzeData)
                    .ToList()
                    .Select(x => MapToEskaeraDTO(x, erabiltzaileak))
                    .ToList();

                var data = new OdooSyncDTO
                {
                    zerbitzariak = zerbitzariak,
                    platerak = platerak,
                    mahaiak = mahaiak,
                    eskaerak = eskaerak,
                    fakturak = fakturak
                };

                return (true, null, data);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        private static OdooZerbitzariaDTO MapToZerbitzariaDTO(Erabiltzailea erabiltzailea)
        {
            return new OdooZerbitzariaDTO
            {
                id = erabiltzailea.id,
                izena = erabiltzailea.erabiltzailea,
                emaila = erabiltzailea.emaila ?? string.Empty,
                rolaIzena = erabiltzailea.rola?.izena ?? string.Empty,
                txat = erabiltzailea.txat
            };
        }

        private static OdooProduktuaDTO MapToProduktuaDTO(Produktua produktua)
        {
            return new OdooProduktuaDTO
            {
                id = produktua.id,
                izena = produktua.izena,
                prezioa = produktua.prezioa,
                kategoriaId = produktua.kategoria?.id ?? 0,
                stockAktuala = produktua.stock_aktuala
            };
        }

        private static OdooMahaiaDTO MapToMahaiaDTO(Mahaia mahaia)
        {
            return new OdooMahaiaDTO
            {
                id = mahaia.id,
                zenbakia = mahaia.zenbakia,
                kapazitatea = mahaia.kapazitatea,
                egoera = mahaia.egoera ?? string.Empty
            };
        }

        private static FakturaDTO MapToFakturaDTO(Faktura faktura)
        {
            return new FakturaDTO
            {
                Id = faktura.id,
                EskaeraId = faktura.eskaeraId,
                PdfIzena = faktura.pdfIzena ?? string.Empty,
                Data = faktura.data,
                Totala = faktura.guztira ?? 0
            };
        }

        private static OdooEskaeraDTO MapToEskaeraDTO(Eskaera eskaera, IDictionary<int, string> erabiltzaileak)
        {
            return new OdooEskaeraDTO
            {
                id = eskaera.id,
                izena = $"Eskaera {eskaera.id}",
                mahaiaId = eskaera.mahaia_id ?? 0,
                erabiltzaileId = eskaera.erabiltzaileId,
                erabiltzaileIzena = erabiltzaileak.TryGetValue(eskaera.erabiltzaileId, out var izena)
                    ? izena
                    : string.Empty,
                komensalak = eskaera.komensalak,
                egoera = eskaera.egoera ?? string.Empty,
                sukaldeaEgoera = eskaera.sukaldeaEgoera ?? string.Empty,
                sortzeData = eskaera.sortzeData,
                itxieraData = eskaera.itxieraData,
                txanda = string.IsNullOrWhiteSpace(eskaera.txanda)
                    ? InferituTxanda(eskaera.sortzeData)
                    : FormateatuTxanda(eskaera.txanda),
                lerroak = eskaera.EskaeraProduktuak
                    .Select(MapToEskaeraLerroDTO)
                    .ToList()
            };
        }

        private static string FormateatuTxanda(string? txanda)
        {
            return string.Equals(txanda, "afaria", StringComparison.OrdinalIgnoreCase)
                ? "Afaria"
                : "Bazkaria";
        }

        private static OdooEskaeraLerroDTO MapToEskaeraLerroDTO(EskaeraProduktuak lerroa)
        {
            var lineTotal = lerroa.Guztira;
            if (lineTotal <= 0)
            {
                lineTotal = lerroa.PrezioUnitarioa * lerroa.Kantitatea;
            }

            return new OdooEskaeraLerroDTO
            {
                id = lerroa.Id,
                produktuaId = lerroa.Produktua.id,
                produktuaIzena = lerroa.Produktua.izena,
                kantitatea = lerroa.Kantitatea,
                prezioUnitarioa = lerroa.PrezioUnitarioa,
                guztira = lineTotal
            };
        }

        private static string InferituTxanda(DateTime data)
        {
            return data.Hour >= 18 ? "Afaria" : "Bazkaria";
        }
    }
}
