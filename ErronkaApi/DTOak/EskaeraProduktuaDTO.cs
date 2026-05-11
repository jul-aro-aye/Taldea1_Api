namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskari bati lotutako produktu baten informazioa.
    /// </summary>
    /// <remarks>
    /// Produktua, prezioa eta kantitatea erakusten ditu.
    /// </remarks>
    public class EskaeraProduktuaDTO
    {
        /// <summary>
        /// Produktuaren zenbakia.
        /// </summary>
        public int ProduktuaId { get; set; }

        /// <summary>
        /// Produktuaren prezioa.
        /// </summary>
        public decimal PrezioUnitarioa { get; set; }

        /// <summary>
        /// Zenbat produktu dauden.
        /// </summary>
        public int Kantitatea { get; set; }
    }
}
