namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Produktu baten informazioa.
    /// </summary>
    /// <remarks>
    /// Produktuak ikusteko, sortzeko edo aldatzeko erabiltzen da.
    /// </remarks>
    public class ProduktuaDTO
    {
        /// <summary>
        /// Produktuaren zenbakia.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Produktuaren izena.
        /// </summary>
        public string izena { get; set; }

        /// <summary>
        /// Produktuaren prezioa.
        /// </summary>
        public decimal prezioa { get; set; }

        /// <summary>
        /// Kategoriaren zenbakia.
        /// </summary>
        public int kategoria_id { get; set; }

        /// <summary>
        /// Stock-ean dagoen kopurua.
        /// </summary>
        public int stock_aktuala { get; set; }
    }
}
