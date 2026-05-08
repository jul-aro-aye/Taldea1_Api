namespace ErronkaApi.DTOak
{
    public class EskaeraDTO
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public int MahaiaId { get; set; }
        public int Komensalak { get; set; }
        public string Egoera { get; set; }
        public string Data { get; set; }
        public string Txanda { get; set; }
        public string SukaldeaEgoera { get; set; }
        public string? DeskontuKodea { get; set; }
        public decimal DeskontuPortzentaia { get; set; }
    }
}
