using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate;
using NHibernate.Linq;

namespace ErronkaApi.Repositorioak
{
    public class ErreserbaRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public ErreserbaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public (bool success, string? error, List<ErreserbaDTO>? data) LortuErreserbak()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var lista = session.Query<Erreserba>()
                    .OrderBy(e => e.erreserbaData)
                    .ThenBy(e => e.txanda)
                    .ThenBy(e => e.id)
                    .ToList()
                    .Select(MapToDTO)
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error, List<ErreserbaDTO>? data) LortuErreserbakDatarenArabera(DateTime data)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var hasiera = data.Date;
                var amaiera = hasiera.AddDays(1);

                var lista = session.Query<Erreserba>()
                    .Where(e => e.erreserbaData >= hasiera && e.erreserbaData < amaiera)
                    .OrderBy(e => e.txanda)
                    .ThenBy(e => e.id)
                    .ToList()
                    .Select(MapToDTO)
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error, ErreserbaDTO? data) SortuErreserba(ErreserbaDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var (baliozkoa, errorea, _) = BalidatuErreserba(session, dto, null);
                if (!baliozkoa)
                    return (false, errorea, null);

                var erreserba = new Erreserba
                {
                    mahaiaId = dto.MahaiaId,
                    bezeroaIzena = dto.Izena,
                    telefonoa = dto.Telefonoa,
                    data = DateTime.Today,
                    erreserbaData = dto.Data.Date,
                    txanda = NormalizeTxanda(dto.Txanda),
                    pertsonaKopurua = dto.PertsonaKopurua,
                    egoera = string.IsNullOrWhiteSpace(dto.Egoera) ? "sortua" : dto.Egoera
                };

                session.Save(erreserba);
                tx.Commit();

                return (true, null, MapToDTO(erreserba));
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error, ErreserbaDTO? data) EguneratuErreserba(int id, ErreserbaDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var erreserba = session.Get<Erreserba>(id);
                if (erreserba == null)
                    return (false, "Erreserba ez da aurkitu", null);

                var mahaiaId = dto.MahaiaId > 0 ? dto.MahaiaId : erreserba.mahaiaId;
                var dtoEguneratua = new ErreserbaDTO
                {
                    Id = id,
                    MahaiaId = mahaiaId,
                    Izena = dto.Izena,
                    Telefonoa = dto.Telefonoa,
                    Txanda = dto.Txanda,
                    PertsonaKopurua = dto.PertsonaKopurua,
                    Data = dto.Data,
                    Egoera = dto.Egoera
                };

                var (baliozkoa, errorea, mahaia) = BalidatuErreserba(session, dtoEguneratua, id);
                if (!baliozkoa)
                    return (false, errorea, null);

                erreserba.mahaiaId = mahaia!.id;

                erreserba.bezeroaIzena = dto.Izena;
                erreserba.telefonoa = dto.Telefonoa;
                erreserba.erreserbaData = dto.Data.Date;
                erreserba.txanda = NormalizeTxanda(dto.Txanda);
                erreserba.pertsonaKopurua = dto.PertsonaKopurua;

                if (!string.IsNullOrWhiteSpace(dto.Egoera))
                    erreserba.egoera = dto.Egoera;

                session.Update(erreserba);
                tx.Commit();

                return (true, null, MapToDTO(erreserba));
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error) EzabatuErreserba(int id)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var erreserba = session.Get<Erreserba>(id);
                if (erreserba == null)
                    return (false, "Erreserba ez da aurkitu");

                session.Delete(erreserba);
                tx.Commit();
                return (true, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message);
            }
        }

        public (bool success, string? error, int? data) EzarriMahaia(int erreserbaId, int mahaiaId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var erreserba = session.Get<Erreserba>(erreserbaId);
                if (erreserba == null)
                    return (false, "Erreserba ez da aurkitu", null);

                var mahaia = session.Get<Mahaia>(mahaiaId);
                if (mahaia == null)
                    return (false, "Mahaia ez da aurkitu", null);

                if (mahaia.kapazitatea < erreserba.pertsonaKopurua)
                    return (false, "Mahaia txikiegia da erreserbarentzat", null);

                var txandaNormalizatua = NormalizeTxanda(erreserba.txanda);
                var besteErreserbaBat = session.Query<Erreserba>()
                    .FirstOrDefault(e =>
                        e.id != erreserbaId &&
                        e.mahaiaId == mahaiaId &&
                        e.erreserbaData == erreserba.erreserbaData &&
                        e.txanda == txandaNormalizatua &&
                        e.egoera != "bertan_behera");

                if (besteErreserbaBat != null)
                    return (false, "Mahaia jada erreserbatuta dago data eta txanda horretan", null);

                erreserba.mahaiaId = mahaiaId;
                session.Update(erreserba);
                tx.Commit();

                return (true, null, mahaiaId);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message, null);
            }
        }

        private static (bool success, string? error, Mahaia? mahaia) BalidatuErreserba(
            global::NHibernate.ISession session,
            ErreserbaDTO dto,
            int? unekoErreserbaId)
        {
            if (dto.MahaiaId <= 0)
                return (false, "Mahaia beharrezkoa da", null);

            if (string.IsNullOrWhiteSpace(dto.Izena))
                return (false, "Bezeroaren izena beharrezkoa da", null);

            if (dto.PertsonaKopurua <= 0)
                return (false, "Pertsona kopurua ez da zuzena", null);

            if (dto.Data == default)
                return (false, "Erreserba data beharrezkoa da", null);

            var mahaia = session.Get<Mahaia>(dto.MahaiaId);
            if (mahaia == null)
                return (false, "Mahaia ez da aurkitu", null);

            if (mahaia.kapazitatea < dto.PertsonaKopurua)
                return (false, "Mahaia txikiegia da pertsona kopuru horretarako", null);

            var txandaNormalizatua = NormalizeTxanda(dto.Txanda);
            var query = session.Query<Erreserba>()
                .Where(e =>
                    e.mahaiaId == dto.MahaiaId &&
                    e.erreserbaData == dto.Data.Date &&
                    e.txanda == txandaNormalizatua &&
                    e.egoera != "bertan_behera");

            if (unekoErreserbaId.HasValue)
                query = query.Where(e => e.id != unekoErreserbaId.Value);

            var erreserbaExistentea = query.FirstOrDefault();

            if (erreserbaExistentea != null)
                return (false, "Mahaia jada erreserbatuta dago data eta txanda horretan", null);

            return (true, null, mahaia);
        }

        public (bool success, string? error, List<int>? data) LortuMahaiakErreserbarentzat(int erreserbaId)
        {
            try
            {
                using var session = _sessionFactory.OpenSession();
                var erreserba = session.Get<Erreserba>(erreserbaId);

                if (erreserba == null)
                    return (false, "Erreserba ez da aurkitu", null);

                return (true, null, new List<int> { erreserba.mahaiaId });
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        private static ErreserbaDTO MapToDTO(Erreserba erreserba)
        {
            return new ErreserbaDTO
            {
                Id = erreserba.id,
                MahaiaId = erreserba.mahaiaId,
                Izena = erreserba.bezeroaIzena,
                Telefonoa = erreserba.telefonoa,
                Txanda = FormatTxanda(erreserba.txanda),
                PertsonaKopurua = erreserba.pertsonaKopurua,
                Data = erreserba.erreserbaData,
                Egoera = erreserba.egoera
            };
        }

        private static string NormalizeTxanda(string? txanda)
        {
            if (string.Equals(txanda, "Afaria", StringComparison.OrdinalIgnoreCase))
                return "afaria";

            return "bazkaria";
        }

        private static string FormatTxanda(string? txanda)
        {
            if (string.Equals(txanda, "afaria", StringComparison.OrdinalIgnoreCase))
                return "Afaria";

            return "Bazkaria";
        }
    }
}
