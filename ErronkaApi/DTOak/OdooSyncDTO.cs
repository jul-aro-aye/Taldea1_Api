namespace ErronkaApi.DTOak
{
    /// <summary>
    /// APIa eta Odoo sinkronizatzeko datu guztiak.
    /// </summary>
    /// <remarks>
    /// Proiektuko datu guztiak pakete batean bidaltzeko Odoo-ra.
    /// </remarks>
    public class OdooSyncDTO
    {
        /// <summary>
        /// Odoo-ra bidaltzeko erabiltzaileak.
        /// </summary>
        public List<OdooZerbitzariaDTO> zerbitzariak { get; set; } = new();

        /// <summary>
        /// Odoo-ra bidaltzeko produktuak.
        /// </summary>
        public List<OdooProduktuaDTO> platerak { get; set; } = new();

        /// <summary>
        /// Odoo-ra bidaltzeko mahaiak.
        /// </summary>
        public List<OdooMahaiaDTO> mahaiak { get; set; } = new();

        /// <summary>
        /// Odoo-ra bidaltzeko eskariak.
        /// </summary>
        public List<OdooEskaeraDTO> eskaerak { get; set; } = new();

        /// <summary>
        /// Odoo-ra bidaltzeko fakturak.
        /// </summary>
        public List<FakturaDTO> fakturak { get; set; } = new();
    }

    /// <summary>
    /// Odoo-rako egokitutako erabiltzailea.
    /// </summary>
    /// <remarks>
    /// Kanpoko sistemarekin integratzeko datuak bakarrik erakusten ditu.
    /// </remarks>
    public class OdooZerbitzariaDTO
    {
        /// <summary>
        /// Erabiltzailearen zenbakia.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Erabiltzailearen izena.
        /// </summary>
        public string izena { get; set; } = string.Empty;

        /// <summary>
        /// Posta elektronikoa.
        /// </summary>
        public string emaila { get; set; } = string.Empty;

        /// <summary>
        /// Rolaren izena.
        /// </summary>
        public string rolaIzena { get; set; } = string.Empty;

        /// <summary>
        /// Txata erabili ahal duen ala ez.
        /// </summary>
        public bool txat { get; set; }
    }

    /// <summary>
    /// Odoo-rako egokitutako produktua.
    /// </summary>
    /// <remarks>
    /// Produktuen katalogoa kanpoko sistemara bidaltzeko.
    /// </remarks>
    public class OdooProduktuaDTO
    {
        /// <summary>
        /// Produktuaren zenbakia.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Produktuaren izena.
        /// </summary>
        public string izena { get; set; } = string.Empty;

        /// <summary>
        /// Produktuaren prezioa.
        /// </summary>
        public decimal prezioa { get; set; }

        /// <summary>
        /// Kategoriaren zenbakia.
        /// </summary>
        public int kategoriaId { get; set; }

        /// <summary>
        /// Produktuaren stock-a.
        /// </summary>
        public int stockAktuala { get; set; }
    }

    /// <summary>
    /// Odoo-rako egokitutako mahaia.
    /// </summary>
    /// <remarks>
    /// Mahaiaren oinarrizko informazioa Odoo-ra bidaltzeko.
    /// </remarks>
    public class OdooMahaiaDTO
    {
        /// <summary>
        /// Mahaiaren zenbakia (ID).
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Mahaiaren zenbakia.
        /// </summary>
        public int zenbakia { get; set; }

        /// <summary>
        /// Mahaian zenbat pertsona kabitzen diren.
        /// </summary>
        public int kapazitatea { get; set; }

        /// <summary>
        /// Mahaiaren egoera.
        /// </summary>
        public string egoera { get; set; } = string.Empty;
    }

    /// <summary>
    /// Odoo-rako egokitutako eskaria.
    /// </summary>
    /// <remarks>
    /// Eskaria, mahaia, erabiltzailea eta produktuak lotzen ditu Odoo-rako.
    /// </remarks>
    public class OdooEskaeraDTO
    {
        /// <summary>
        /// Eskariaren zenbakia.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Eskariaren izena.
        /// </summary>
        public string izena { get; set; } = string.Empty;

        /// <summary>
        /// Mahaiaren zenbakia.
        /// </summary>
        public int mahaiaId { get; set; }

        /// <summary>
        /// Erabiltzailearen zenbakia.
        /// </summary>
        public int erabiltzaileId { get; set; }

        /// <summary>
        /// Erabiltzailearen izena.
        /// </summary>
        public string erabiltzaileIzena { get; set; } = string.Empty;

        /// <summary>
        /// Zenbat pertsona dauden mahaian.
        /// </summary>
        public int komensalak { get; set; }

        /// <summary>
        /// Eskariaren egoera orokorra.
        /// </summary>
        public string egoera { get; set; } = string.Empty;

        /// <summary>
        /// Sukaldeko egoera.
        /// </summary>
        public string sukaldeaEgoera { get; set; } = string.Empty;

        /// <summary>
        /// Eskaria ireki den eguna.
        /// </summary>
        public DateTime sortzeData { get; set; }

        /// <summary>
        /// Eskaria itxi den eguna.
        /// </summary>
        public DateTime? itxieraData { get; set; }

        /// <summary>
        /// Bazkaria edo afaria den.
        /// </summary>
        public string txanda { get; set; } = string.Empty;

        /// <summary>
        /// Eskarian dauden produktuak.
        /// </summary>
        public List<OdooEskaeraLerroDTO> lerroak { get; set; } = new();
    }

    /// <summary>
    /// Odoo-rako egokitutako eskari-lerroa.
    /// </summary>
    /// <remarks>
    /// Eskari bateko produktu baten informazioa Odoo-rako.
    /// </remarks>
    public class OdooEskaeraLerroDTO
    {
        /// <summary>
        /// Eskari-lerroaren zenbakia.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Produktuaren zenbakia.
        /// </summary>
        public int produktuaId { get; set; }

        /// <summary>
        /// Produktuaren izena.
        /// </summary>
        public string produktuaIzena { get; set; } = string.Empty;

        /// <summary>
        /// Zenbat produktu dauden.
        /// </summary>
        public int kantitatea { get; set; }

        /// <summary>
        /// Produktu baten prezioa.
        /// </summary>
        public decimal prezioUnitarioa { get; set; }

        /// <summary>
        /// Guztira balio duena.
        /// </summary>
        public decimal guztira { get; set; }
    }
}
