namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskari bat aldatzeko datuak.
    /// </summary>
    /// <remarks>
    /// Eskarian dauden pertsonak eta produktuak aldatzeko balio du.
    /// </remarks>
    public class EskaeraEguneratuDTO
    {
        /// <summary>
        /// Pertsona kopuru berria.
        /// </summary>
        public int Komensalak { get; set; }

        /// <summary>
        /// Eskarian geratuko diren produktuen zerrenda.
        /// </summary>
        public List<EskaeraProduktuaEditatuDTO> Produktuak { get; set; }
    }
}
