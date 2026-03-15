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

        public MahaiaRepository() { }

        public virtual (bool success, string? error, List<MahaiaDTO>? data) LortuMahaiLibre()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var lista = session.Query<Mahaia>()
                    .Where(m => m.egoera == "libre")
                    .Select(MapToDTO)
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error, MahaiaDTO? data) LortuMahaiBat(int id)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var mahaia = session.Get<Mahaia>(id);

                if (mahaia == null)
                    return (false, "Mahaia ez da existitzen", null);

                return (true, null, MapToDTO(mahaia));
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        private MahaiaDTO MapToDTO(Mahaia m)
        {
            return new MahaiaDTO
            {
                Id = m.id,
                Zenbakia = m.zenbakia,
                kapazitatea = m.kapazitatea
            };
        }
    }
}
