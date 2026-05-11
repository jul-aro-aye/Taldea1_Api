namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Eskaria ordaintzeko informazioa.
    /// </summary>
    /// <remarks>
    /// Ordainketan deskontuak aplikatzeko erabiltzen da.
    /// </remarks>
    public class EskaeraOrdainduDTO
    {
        /// <summary>
        /// Deskontu kodea.
        /// </summary>
        public string? DeskontuKodea { get; set; }

        /// <summary>
        /// Deskontu ehunekoa.
        /// </summary>
        public decimal? DeskontuPortzentaia { get; set; }
    }
}
