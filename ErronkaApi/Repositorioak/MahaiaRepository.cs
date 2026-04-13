using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;

namespace ErronkaApi.Repositorioak
{
    public class MahaiaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public MahaiaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public MahaiaRepository()
        {

        }

        public virtual (bool success, string? error, List<MahaiaDTO>? data) LortuMahaiak(DateTime? data = null, string? txanda = null)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var mahaiOkupatuak = LortuMahaiOkupatuenIdak(session, data, txanda);

                var lista = session.Query<Mahaia>()
                    .OrderBy(m => m.zenbakia)
                    .ToList()
                    .Select(m => MapToDTO(m, mahaiOkupatuak))
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error, List<MahaiaDTO>? data) LortuMahaiLibre(DateTime? data = null, string? txanda = null)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var mahaiOkupatuak = LortuMahaiOkupatuenIdak(session, data, txanda);

                var lista = session.Query<Mahaia>()
                    .ToList()
                    .Where(m => !mahaiOkupatuak.Contains(m.id))
                    .Select(m => MapToDTO(m, mahaiOkupatuak))
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error, MahaiaDTO? data) LortuMahaiBat(int id, DateTime? data = null, string? txanda = null)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var mahaiOkupatuak = LortuMahaiOkupatuenIdak(session, data, txanda);
                var mahaia = session.Get<Mahaia>(id);

                if (mahaia == null)
                    return (false, "Mahaia ez da existitzen", null);

                return (true, null, MapToDTO(mahaia, mahaiOkupatuak));
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        private static string NormalizeTxanda(string? txanda)
        {
            if (string.Equals(txanda, "Afaria", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(txanda, "afaria", StringComparison.OrdinalIgnoreCase))
                return "afaria";

            return "bazkaria";
        }

        private static string InferituTxanda(DateTime data)
            => data.Hour >= 18 ? "afaria" : "bazkaria";

        private static bool EskaeraDagokioDataTxandari(
            global::NHibernate.ISession session,
            Eskaera eskaera,
            DateTime data,
            string txanda)
        {
            if (eskaera.erreserbaId.HasValue)
            {
                var erreserba = session.Get<Erreserba>(eskaera.erreserbaId.Value);
                if (erreserba != null)
                {
                    return erreserba.erreserbaData.Date == data.Date &&
                           NormalizeTxanda(erreserba.txanda) == NormalizeTxanda(txanda);
                }
            }

            return eskaera.sortzeData.Date == data.Date &&
                   InferituTxanda(eskaera.sortzeData) == NormalizeTxanda(txanda);
        }

        private static HashSet<int> LortuMahaiOkupatuenIdak(
            global::NHibernate.ISession session,
            DateTime? data = null,
            string? txanda = null)
        {
            var eskaerak = session.Query<Eskaera>()
                .Where(e =>
                    e.mahaia_id.HasValue &&
                    (e.egoera == "irekita" || e.egoera == "ordainketa_pendiente"))
                .ToList();

            if (!data.HasValue)
                return eskaerak.Select(e => e.mahaia_id!.Value).ToHashSet();

            var dataErabilgarria = data.Value.Date;
            var txandaErabilgarria = NormalizeTxanda(txanda);

            return eskaerak
                .Where(e => EskaeraDagokioDataTxandari(session, e, dataErabilgarria, txandaErabilgarria))
                .Select(e => e.mahaia_id!.Value)
                .ToHashSet();
        }

        private MahaiaDTO MapToDTO(Mahaia m, HashSet<int> mahaiOkupatuak)
        {
            return new MahaiaDTO
            {
                Id = m.id,
                Zenbakia = m.zenbakia,
                kapazitatea = m.kapazitatea > 0 ? m.kapazitatea : 0,
                Egoera = mahaiOkupatuak.Contains(m.id) ? "okupatuta" : "libre"
            };
        }
    }
}
