namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Erreserba bat eta mahai baten arteko lotura.
    /// </summary>
    /// <remarks>
    /// Erreserba bat zein mahaitan dagoen azaltzen du.
    /// </remarks>
    public class ErreserbaMahaiDTO
    {
        /// <summary>
        /// Loturaren zenbakia.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Erreserbaren zenbakia.
        /// </summary>
        public int ErreserbakId { get; set; }

        /// <summary>
        /// Mahaiaren zenbakia.
        /// </summary>
        public int MahaiakId { get; set; }
    }
}
