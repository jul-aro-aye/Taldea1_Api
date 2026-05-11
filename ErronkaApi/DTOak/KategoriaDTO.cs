namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Produktu kategoria baten informazioa.
    /// </summary>
    /// <remarks>
    /// Kategoriak ikusteko edo kudeatzeko erabiltzen da.
    /// </remarks>
    public class KategoriaDTO
    {
        /// <summary>
        /// Kategoriaren zenbakia.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Kategoriaren izena.
        /// </summary>
        public string izena { get; set; }
    }
}
