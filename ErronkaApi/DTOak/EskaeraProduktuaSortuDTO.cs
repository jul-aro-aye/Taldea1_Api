namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskari berri batean produktuak sartzeko datuak.
    /// </summary>
    /// <remarks>
    /// Eskaria sortzean produktu bakoitzaren zenbakia, kantitatea eta prezioa behar dira.
    /// </remarks>
    public class EskaeraProduktuaSortuDTO
    {
        /// <summary>
        /// Produktuaren zenbakia.
        /// </summary>
        public int ProduktuaId { get; set; }

        /// <summary>
        /// Zenbat produktu nahi diren.
        /// </summary>
        public int Kantitatea { get; set; }

        /// <summary>
        /// Produktuaren prezioa.
        /// </summary>
        public decimal PrezioUnitarioa { get; set; }
    }
}
