namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Faktura baten informazioa.
    /// </summary>
    /// <remarks>
    /// Faktura, eskaria, erreserba eta PDF fitxategia lotzen ditu.
    /// </remarks>
    public class FakturaDTO
    {
        /// <summary>
        /// Fakturaren zenbakia.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Eskariaren zenbakia.
        /// </summary>
        public int EskaeraId { get; set; }

        /// <summary>
        /// Erreserbaren zenbakia (egon ezkero).
        /// </summary>
        public int? ErreserbaId { get; set; }

        /// <summary>
        /// PDF fitxategiaren izena.
        /// </summary>
        public string PdfIzena { get; set; }

        /// <summary>
        /// Fakturaren eguna.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Fakturaren guztizko zenbatekoa.
        /// </summary>
        public decimal Totala { get; set; }
    }
}
