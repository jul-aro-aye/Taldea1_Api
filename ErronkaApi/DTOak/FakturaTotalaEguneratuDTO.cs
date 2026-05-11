namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Faktura baten guztizko zenbatekoa aldatzeko datuak.
    /// </summary>
    /// <remarks>
    /// Faktura bati diru kopuru bat gehitzeko erabiltzen da.
    /// </remarks>
    public class FakturaTotalaEguneratuDTO
    {
        /// <summary>
        /// Aldatu nahi den fakturaren zenbakia.
        /// </summary>
        public int FakturaId { get; set; }

        /// <summary>
        /// Gehitu nahi den diru kopurua.
        /// </summary>
        public decimal Gehikuntza { get; set; }
    }
}
