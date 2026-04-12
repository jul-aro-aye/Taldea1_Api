namespace ErronkaApi.DTOak
{
    public class ErreserbaDTO
    {
        public int Id { get; set; }
        public int MahaiaId { get; set; }
        public string Izena { get; set; }
        public string? Telefonoa { get; set; }
        public string Txanda { get; set; }
        public int PertsonaKopurua { get; set; }
        public DateTime Data { get; set; }
        public string Egoera { get; set; }
    }
}
