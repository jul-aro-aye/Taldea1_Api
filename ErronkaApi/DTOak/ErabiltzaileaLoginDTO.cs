namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Saioa hasteko erabiltzailearen informazioa.
    /// </summary>
    /// <remarks>
    /// DTO honek erabiltzailearen datu garrantzitsuak bakarrik erakusten ditu.
    /// </remarks>
    public class ErabiltzaileaLoginDTO
    {
        /// <summary>
        /// Erabiltzailearen zenbakia.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Erabiltzaile izena.
        /// </summary>
        public string erabiltzailea { get; set; }

        /// <summary>
        /// Posta elektronikoa.
        /// </summary>
        public string emaila { get; set; }

        /// <summary>
        /// Rolaren zenbakia.
        /// </summary>
        public int rolaId { get; set; }

        /// <summary>
        /// Rolaren izena.
        /// </summary>
        public string rolaIzena { get; set; }

        /// <summary>
        /// Txata erabili ahal duen ala ez.
        /// </summary>
        public bool txat { get; set; }
    }
}
