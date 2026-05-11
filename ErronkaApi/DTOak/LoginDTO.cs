namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Saioa hasteko kredentzialak.
    /// </summary>
    /// <remarks>
    /// Erabiltzailea eta pasahitza egiaztatzeko erabiltzen da.
    /// </remarks>
    public class LoginDTO
    {
        /// <summary>
        /// Erabiltzaile izena.
        /// </summary>
        public string erabiltzailea { get; set; }

        /// <summary>
        /// Pasahitza.
        /// </summary>
        public string pasahitza { get; set; }

        /// <summary>
        /// Txata aktibatuta dagoen ala ez.
        /// </summary>
        public Boolean txat { get; set; }
    }
}
