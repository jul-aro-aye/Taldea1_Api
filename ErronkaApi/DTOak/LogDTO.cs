namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Ekintza baten log-a gordetzeko datuak.
    /// </summary>
    /// <remarks>
    /// Erabiltzaileak zer ekintza egin duen gordetzeko balio du.
    /// </remarks>
    public class LogDTO
    {
        /// <summary>
        /// Ekintza egin duen erabiltzailearen zenbakia.
        /// </summary>
        public int Erabiltzailea { get; set; }

        /// <summary>
        /// Egindako ekintzaren azalpena.
        /// </summary>
        public string Ekintza { get; set; }
    }
}
