namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Erreserba baten informazioa.
    /// </summary>
    /// <remarks>
    /// Erreserbak sortzeko, aldatzeko edo ikusteko erabiltzen da.
    /// </remarks>
    public class ErreserbaDTO
    {
        /// <summary>
        /// Erreserbaren zenbakia.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mahaiaren zenbakia.
        /// </summary>
        public int MahaiaId { get; set; }

        /// <summary>
        /// Erreserba egin duenaren izena.
        /// </summary>
        public string Izena { get; set; }

        /// <summary>
        /// Kontakturako telefonoa.
        /// </summary>
        public string? Telefonoa { get; set; }

        /// <summary>
        /// Bazkaria edo afaria den.
        /// </summary>
        public string Txanda { get; set; }

        /// <summary>
        /// Zenbat pertsona datozen.
        /// </summary>
        public int PertsonaKopurua { get; set; }

        /// <summary>
        /// Erreserba eguna.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Erreserba nola dagoen.
        /// </summary>
        public string Egoera { get; set; }
    }
}
