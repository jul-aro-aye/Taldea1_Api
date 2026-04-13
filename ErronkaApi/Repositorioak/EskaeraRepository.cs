using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using NHibernate.Linq;
using NHSession = NHibernate.ISession;
using NHFactory = NHibernate.ISessionFactory;
using NHibernate;

namespace ErronkaApi.Repositorioak
{
    public class EskaeraRepository
    {
        private readonly NHFactory _sessionFactory;

        public EskaeraRepository(NHFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        private Eskaera? GetEskaera(NHSession session, int id)
            => session.Get<Eskaera>(id);

        private Produktua? GetProduktua(NHSession session, int id)
            => session.Get<Produktua>(id, LockMode.Upgrade);

        private Mahaia? GetMahaia(NHSession session, int id)
            => session.Get<Mahaia>(id);

        private EskaeraDTO MapToEskaeraDTO(Eskaera e)
        {
            return new EskaeraDTO
            {
                Id = e.id,
                Izena = $"Eskaera #{e.id}",
                MahaiaId = e.mahaia_id ?? 0,
                Komensalak = e.komensalak,
                Data = e.sortzeData.ToString("yyyy-MM-dd HH:mm"),
                SukaldeaEgoera = e.sukaldeaEgoera
            };
        }

        private EskaeraLortuDTO MapToEskaeraLortuDTO(EskaeraProduktuak ep)
        {
            return new EskaeraLortuDTO
            {
                ProduktuaId = ep.Produktua.id,
                ProduktuaIzena = ep.Produktua.izena,
                PrezioUnitarioa = ep.PrezioUnitarioa,
                Kantitatea = 1
            };
        }

        public (bool success, string? error, int? data, List<string>? details)
            SortuEskaera(EskaeraSortuDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var mahaia = GetMahaia(session, dto.MahaiaId);
                if (mahaia == null)
                    return (false, "Mahaia ez da aurkitu", null, null);

                if (!string.Equals(mahaia.egoera, "libre", StringComparison.OrdinalIgnoreCase))
                    return (false, "Mahaia ez dago libre", null, null);

                if (dto.ErreserbaId.HasValue)
                {
                    var erreserba = session.Get<Erreserba>(dto.ErreserbaId.Value);

                    if (erreserba == null)
                        return (false, "Erreserba ez da aurkitu", null, null);

                    if (erreserba.mahaiaId != dto.MahaiaId)
                        return (false, "Erreserba ez dator bat aukeratutako mahaiarekin", null, null);
                }

                mahaia.egoera = "okupatuta";
                session.Update(mahaia);

                var faltan = new List<string>();

                foreach (var p in dto.Produktuak)
                {
                    var produktua = GetProduktua(session, p.ProduktuaId);
                    if (produktua == null)
                    {
                        faltan.Add($"Produktua {p.ProduktuaId}");
                        continue;
                    }

                    if (produktua.stock_aktuala < p.Kantitatea)
                        faltan.Add(produktua.izena);
                }

                if (faltan.Any())
                    return (false, "Stock gabe dauden produktuak daude", null, faltan);

                var eskaera = new Eskaera
                {
                    erabiltzaileId = dto.ErabiltzaileId,
                    komensalak = dto.Komensalak,
                    egoera = "irekita",
                    sukaldeaEgoera = "bidalita",
                    sortzeData = DateTime.Now,
                    mahaia_id = dto.MahaiaId,
                    erreserbaId = dto.ErreserbaId
                };

                session.Save(eskaera);

                session.Save(new EskaeraMahaiak
                {
                    Eskaera = eskaera,
                    Mahaia = mahaia
                });

                foreach (var p in dto.Produktuak)
                {
                    var produktua = GetProduktua(session, p.ProduktuaId);
                    if (produktua == null)
                        continue;

                    produktua.stock_aktuala -= p.Kantitatea;
                    session.Update(produktua);

                    session.Save(new EskaeraProduktuak
                    {
                        Eskaera = eskaera,
                        Produktua = produktua,
                        Kantitatea = p.Kantitatea,
                        PrezioUnitarioa = produktua.prezioa,
                        Guztira = produktua.prezioa * p.Kantitatea
                    });
                }

                var guztira = dto.Produktuak.Sum(p =>
                {
                    var produktua = GetProduktua(session, p.ProduktuaId);
                    return produktua == null ? 0 : produktua.prezioa * p.Kantitatea;
                });

                session.Save(new Faktura
                {
                    eskaeraId = eskaera.id,
                    pdfIzena = string.Empty,
                    data = DateTime.Now,
                    guztira = guztira
                });

                tx.Commit();
                return (true, null, eskaera.id, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message, null, null);
            }
        }

        public (bool success, string? error, List<EskaeraDTO>? data)
            LortuEskaerak()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var lista = session.Query<Eskaera>()
                    .Where(e => e.egoera == "irekita")
                    .OrderByDescending(e => e.sortzeData)
                    .Select(MapToEskaeraDTO)
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error, List<EskaeraLortuDTO>? data)
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

        public (bool success, string? error) EzabatuEskaera(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu");

                foreach (var ep in eskaera.EskaeraProduktuak)
                {
                    var produktua = GetProduktua(session, ep.Produktua.id);
                    produktua.stock_aktuala += ep.Kantitatea;
                    session.Update(produktua);
                    session.Delete(ep);
                }

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

        public (bool success, string? error, int? data)
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

        public (bool success, string? error, List<string>? details)
            EguneratuEskaera(int eskaeraId, EskaeraEguneratuDTO dto)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu", null);

                eskaera.komensalak = dto.Komensalak;
                session.Update(eskaera);

                tx.Commit();
                return (true, null, null);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return (false, ex.Message, null);
            }
        }

        public (bool success, string? error)
            EguneratuSukaldeaEgoera(int eskaeraId, string egoera)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
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

        public (bool success, string? error)
            OrdaintzeraBidali(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu");

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

        public (bool success, string? error, string? path)
            SortuFaktura(int eskaeraId)
        {
            using var session = _sessionFactory.OpenSession();
            using var tx = session.BeginTransaction();

            try
            {
                var eskaera = GetEskaera(session, eskaeraId);
                if (eskaera == null)
                    return (false, "Eskaera ez da aurkitu", null);

                var produktuak = session.Query<EskaeraProduktuak>()
                    .Where(p => p.Eskaera.id == eskaeraId)
                    .ToList();
                decimal total = 0;

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
                        decimal prezioa = p.PrezioUnitarioa;
                        int kantitatea = p.Kantitatea;
                        decimal lineaTotala = prezioa * kantitatea;
                        total += lineaTotala;

                        doc.Add(new iTextSharp.text.Paragraph(produktuIzena, normalFont) { SpacingAfter = 1f });
                        doc.Add(new iTextSharp.text.Paragraph($"{kantitatea} x {prezioa:C}    {lineaTotala:C}", normalFont) { SpacingAfter = 3f });
                    }

                    doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph($"TOTALA: {total:C}", titleFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 5f });
                    doc.Add(new iTextSharp.text.Paragraph("Prezioak BEZ barne daude", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_RIGHT, SpacingBefore = 2f });

                    doc.Add(new iTextSharp.text.Paragraph(" ", normalFont));
                    doc.Add(new iTextSharp.text.Paragraph("Enpresaren datuak: NIF: X12345678 | PV: 001", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingBefore = 8f });
                    doc.Add(new iTextSharp.text.Paragraph("ESKERRIK ASKO", normalFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER, SpacingBefore = 8f });

                    doc.Close();
                }

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

                eskaera.egoera = "itxita";
                eskaera.itxieraData = DateTime.Now;
                session.Update(eskaera);

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
                return (false, "Arazoa faktura sortzean: " + ex.Message, null);
            }
        }

        public (bool success, string? error, List<EskaeraDTO>? data)
            LortuEskaerakOrdaintzeko()
        {
            try
            {
                using var session = _sessionFactory.OpenSession();

                var lista = session.Query<Eskaera>()
                    .Where(e => e.egoera == "ordainketa_pendiente")
                    .Select(MapToEskaeraDTO)
                    .ToList();

                return (true, null, lista);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
