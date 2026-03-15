using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;

namespace ErronkaApi.Repositorioak
{
    public class ProduktuaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public ProduktuaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ProduktuaRepository()
        {
            
        }

        public virtual (bool success, string? error, List<ProduktuaDTO>? data) LortuProduktuak()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var lista = session.Query<Produktua>()
                    .Select(MapToDTO)
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error, ProduktuaDTO? data) LortuProduktua(int id)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var produktua = session.Get<Produktua>(id);

                if (produktua == null)
                    return (false, "Produktua ez da aurkitu", null);

                return (true, null, MapToDTO(produktua));
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public virtual (bool success, string? error) GehituProduktua(ProduktuaDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var kategoria = session.Get<Kategoria>(dto.kategoria_id);
                if (kategoria == null)
                    return (false, "Kategoria ez da aurkitu");

                var produktua = new Produktua
                {
                    izena = dto.izena,
                    prezioa = dto.prezioa,
                    kategoria = kategoria,
                    stock_aktuala = dto.stock_aktuala
                };

                session.Save(produktua);
                tx.Commit();

                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual (bool success, string? error) EguneratuProduktua(int id, ProduktuaDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var produktua = session.Get<Produktua>(id);
                if (produktua == null)
                    return (false, "Produktua ez da aurkitu");

                var kategoria = session.Get<Kategoria>(dto.kategoria_id);
                if (kategoria == null)
                    return (false, "Kategoria ez da aurkitu");

                produktua.izena = dto.izena;
                produktua.prezioa = dto.prezioa;
                produktua.kategoria = kategoria;
                produktua.stock_aktuala = dto.stock_aktuala;

                session.Update(produktua);
                tx.Commit();

                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual (bool success, string? error) EzabatuProduktua(int id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var produktua = session.Get<Produktua>(id);
                if (produktua == null)
                    return (false, "Produktua ez da aurkitu");

                session.Delete(produktua);
                tx.Commit();

                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public virtual ProduktuaDTO MapToDTO(Produktua p)
        {
            return new ProduktuaDTO
            {
                id = p.id,
                izena = p.izena,
                prezioa = p.prezioa,
                kategoria_id = p.kategoria.id,
                stock_aktuala = p.stock_aktuala
            };
        }
    }
}
