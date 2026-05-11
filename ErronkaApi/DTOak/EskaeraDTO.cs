namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskari baten informazio orokorra.
    /// </summary>
    /// <remarks>
    /// Eskariak zerrendan ikusteko erabiltzen da.
    /// </remarks>
    public class EskaeraDTO
    {
        /// <summary>
        /// Eskariaren zenbakia.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Eskariaren izena.
        /// </summary>
        public string Izena { get; set; }

        /// <summary>
        /// Mahaiaren zenbakia.
        /// </summary>
        public int MahaiaId { get; set; }

        /// <summary>
        /// Mahaian dauden pertsona kopurua.
        /// </summary>
        public int Komensalak { get; set; }

        /// <summary>
        /// Eskariaren egoera orokorra.
        /// </summary>
        public string Egoera { get; set; }

        /// <summary>
        /// Eskariaren eguna.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Bazkaria edo afaria den.
        /// </summary>
        public string Txanda { get; set; }

        /// <summary>
        /// Sukaldeko egoera.
        /// </summary>
        public string SukaldeaEgoera { get; set; }

        /// <summary>
        /// Erabilitako deskontu kodea.
        /// </summary>
        public string? DeskontuKodea { get; set; }

        /// <summary>
        /// Deskontuaren ehunekoa.
        /// </summary>
        public decimal DeskontuPortzentaia { get; set; }
    }
}
