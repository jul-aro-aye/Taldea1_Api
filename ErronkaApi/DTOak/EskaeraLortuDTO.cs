namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskari baten produktu baten informazioa.
    /// </summary>
    /// <remarks>
    /// Eskari baten barruan dauden produktuak xehetasunez ikusteko.
    /// </remarks>
    public class EskaeraLortuDTO
    {
        /// <summary>
        /// Produktuaren zenbakia.
        /// </summary>
        public int ProduktuaId { get; set; }

        /// <summary>
        /// Produktuaren izena.
        /// </summary>
        public string ProduktuaIzena { get; set; }

        /// <summary>
        /// Produktu baten prezioa.
        /// </summary>
        public decimal PrezioUnitarioa { get; set; }

        /// <summary>
        /// Zenbat produktu dauden.
        /// </summary>
        public int Kantitatea { get; set; }
    }
}
