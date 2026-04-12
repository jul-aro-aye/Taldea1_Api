using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;

namespace ErronkaApi.Repositorioak
{
    public class FakturaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public FakturaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public (bool success, string? error, List<FakturaDTO>? data) LortuFakturak()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var lista = session.Query<Faktura>()
                    .OrderByDescending(f => f.data)
                    .ThenByDescending(f => f.id)
                    .ToList()
                    .Select(f => MapToDTO(session, f))
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error, FakturaDTO? data) LortuFaktura(int id)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var faktura = session.Get<Faktura>(id);

                if (faktura == null)
                    return (false, "Faktura ez da aurkitu", null);

                return (true, null, MapToDTO(session, faktura));
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error, FakturaDTO? data) LortuFakturaErreserbarenArabera(int erreserbaId)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var emaitza = (
                    from f in session.Query<Faktura>()
                    join e in session.Query<Eskaera>() on f.eskaeraId equals e.id
                    where e.erreserbaId == erreserbaId
                    orderby f.data descending, f.id descending
                    select f
                ).FirstOrDefault();

                if (emaitza == null)
                    return (false, "Erreserba horretarako fakturarik ez dago", null);

                return (true, null, MapToDTO(session, emaitza));
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error, FakturaDTO? data) SortuEdoLortuFakturaErreserbatik(int erreserbaId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var eskaera = session.Query<Eskaera>()
                    .FirstOrDefault(e => e.erreserbaId == erreserbaId);

                if (eskaera == null)
                    return (false, "Erreserba horri lotutako eskaerarik ez dago", null);

                var faktura = session.Query<Faktura>()
                    .FirstOrDefault(f => f.eskaeraId == eskaera.id);

                if (faktura == null)
                {
                    faktura = new Faktura
                    {
                        eskaeraId = eskaera.id,
                        pdfIzena = string.Empty,
                        data = DateTime.Now,
                        guztira = KalkulatuTotala(session, eskaera.id)
                    };

                    session.Save(faktura);
                }

                tx.Commit();
                return (true, null, MapToDTO(session, faktura));
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error) EzabatuFaktura(int id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var faktura = session.Get<Faktura>(id);
                if (faktura == null)
                    return (false, "Faktura ez da aurkitu");

                session.Delete(faktura);
                tx.Commit();
                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public (bool success, string? error, FakturaDTO? data) EguneratuTotala(int fakturaId, decimal gehikuntza)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var faktura = session.Get<Faktura>(fakturaId);
                if (faktura == null)
                    return (false, "Faktura ez da aurkitu", null);

                faktura.guztira = (faktura.guztira ?? 0) + gehikuntza;
                faktura.data = DateTime.Now;
                session.Update(faktura);

                tx.Commit();
                return (true, null, MapToDTO(session, faktura));
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message, null);
            }
        }

        private static FakturaDTO MapToDTO(global::NHibernate.ISession session, Faktura faktura)
        {
            var eskaera = session.Query<Eskaera>()
                .FirstOrDefault(e => e.id == faktura.eskaeraId);

            return new FakturaDTO
            {
                Id = faktura.id,
                EskaeraId = faktura.eskaeraId,
                ErreserbaId = eskaera?.erreserbaId,
                PdfIzena = faktura.pdfIzena ?? string.Empty,
                Data = faktura.data,
                Totala = faktura.guztira ?? 0
            };
        }

        private static decimal KalkulatuTotala(global::NHibernate.ISession session, int eskaeraId)
        {
            return session.Query<EskaeraProduktuak>()
                .Where(ep => ep.Eskaera.id == eskaeraId)
                .ToList()
                .Sum(ep => ep.Guztira);
        }
    }
}
