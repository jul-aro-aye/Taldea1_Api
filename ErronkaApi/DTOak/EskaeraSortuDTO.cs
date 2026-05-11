namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskari berri bat irekitzeko datuak.
    /// </summary>
    /// <remarks>
    /// Eskaria sortzeko erabiltzailea, mahaia, pertsona kopurua eta produktuak behar dira.
    /// </remarks>
    public class EskaeraSortuDTO
    {
        /// <summary>
        /// Eskaria egiten duen erabiltzailearen zenbakia.
        /// </summary>
        public int ErabiltzaileId { get; set; }

        /// <summary>
        /// Mahaiaren zenbakia.
        /// </summary>
        public int MahaiaId { get; set; }

        /// <summary>
        /// Zenbat pertsona dauden mahaian.
        /// </summary>
        public int Komensalak { get; set; }

        /// <summary>
        /// Eskariari lotutako erreserba (beharrezkoa ez bada, hutsik).
        /// </summary>
        public int? ErreserbaId { get; set; }

        /// <summary>
        /// Eskariaren eguna.
        /// </summary>
        public DateTime? Data { get; set; }

        /// <summary>
        /// Bazkaria edo afaria den.
        /// </summary>
        public string? Txanda { get; set; }

        /// <summary>
        /// Eskariaren hasierako produktuak.
        /// </summary>
        public List<EskaeraProduktuaSortuDTO> Produktuak { get; set; }
    }
}
