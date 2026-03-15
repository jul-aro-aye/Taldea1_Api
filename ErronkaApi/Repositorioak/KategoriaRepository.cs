using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;

namespace ErronkaApi.Repositorioak
{
    public class KategoriaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public KategoriaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public KategoriaRepository()
        {
            
        }

        public virtual (bool success, string? error, List<KategoriaDTO>? data) LortuKategoriak()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var lista = session.Query<Kategoria>()
                    .Select(MapToDTO)
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error, KategoriaDTO? data) LortuKategoria(int id)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var kategoria = session.Get<Kategoria>(id);

                if (kategoria == null)
                    return (false, "Kategoria ez da aurkitu", null);

                return (true, null, MapToDTO(kategoria));
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error) GehituKategoria(KategoriaDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var kategoria = new Kategoria
                {
                    izena = dto.izena
                };

                session.Save(kategoria);
                tx.Commit();

                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual (bool success, string? error) EguneratuKategoria(int id, KategoriaDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var kategoria = session.Get<Kategoria>(id);
                if (kategoria == null)
                    return (false, "Kategoria ez da aurkitu");

                kategoria.izena = dto.izena;

                session.Update(kategoria);
                tx.Commit();

                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual (bool success, string? error) EzabatuKategoria(int id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var kategoria = session.Get<Kategoria>(id);
                if (kategoria == null)
                    return (false, "Kategoria ez da aurkitu");

                session.Delete(kategoria);
                tx.Commit();

                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual KategoriaDTO MapToDTO(Kategoria k)
        {
            return new KategoriaDTO
            {
                id = k.id,
                izena = k.izena
            };
        }
    }
}
