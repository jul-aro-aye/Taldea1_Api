using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;

namespace ErronkaApi.Repositorioak
{
    public class ErabiltzaileaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public ErabiltzaileaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ErabiltzaileaRepository() { }

        public virtual (bool success, string? error, Erabiltzailea? user) Login(string erabiltzailea, string pasahitza)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var user = session.Query<Erabiltzailea>()
                    .FirstOrDefault(e =>
                        e.erabiltzailea == erabiltzailea &&
                        e.pasahitza == pasahitza &&
                        !e.ezabatua);

                if (user == null)
                    return (false, "Erabiltzailea edo pasahitza okerra", null);

                return (true, null, user);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
