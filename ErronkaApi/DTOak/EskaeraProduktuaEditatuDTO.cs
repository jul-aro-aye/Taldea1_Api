namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskari bateko produktu bat aldatzeko datuak.
    /// </summary>
    /// <remarks>
    /// Eskaria aldatzean produktua eta kantitatea bakarrik bidaltzen dira.
    /// </remarks>
    public class EskaeraProduktuaEditatuDTO
    {
        /// <summary>
        /// Produktuaren zenbakia.
        /// </summary>
        public int ProduktuaId { get; set; }

        /// <summary>
        /// Kantitate berria.
        /// </summary>
        public int Kantitatea { get; set; }
    }
}
