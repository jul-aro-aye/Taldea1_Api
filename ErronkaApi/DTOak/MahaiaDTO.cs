namespace ErronkaApi.DTOak
{
    /// <summary>
    /// Mahai baten informazioa.
    /// </summary>
    /// <remarks>
    /// Mahaiaren zenbakia, lekua eta egoera erakusten ditu.
    /// </remarks>
    public class MahaiaDTO
    {
        /// <summary>
        /// Mahaiaren zenbakia (ID).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mahaiaren zenbakia (jatetxean).
        /// </summary>
        public int Zenbakia { get; set; }

        /// <summary>
        /// Mahaian zenbat pertsona kabitzen diren.
        /// </summary>
        public int kapazitatea { get; set; }

        /// <summary>
        /// Mahaia libre edo hartuta dagoen.
        /// </summary>
        public string Egoera { get; set; }
    }
}
