namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Erabiltzaile berri bat sortzeko datuak.
    /// </summary>
    /// <remarks>
    /// DTO honek pasahitza behar du erabiltzailea sortzeko.
    /// </remarks>
    public class ErabiltzaileaSortuDTO
    {
        /// <summary>
        /// Erabiltzaile izen berria.
        /// </summary>
        public string erabiltzailea { get; set; } = string.Empty;

        /// <summary>
        /// Posta elektroniko berria.
        /// </summary>
        public string emaila { get; set; } = string.Empty;

        /// <summary>
        /// Hasierako pasahitza.
        /// </summary>
        public string pasahitza { get; set; } = string.Empty;

        /// <summary>
        /// Txata erabili ahal duen ala ez.
        /// </summary>
        public bool txat { get; set; }
    }
}
