namespace ErronkaApi.DTOak
{
    public class FakturaDTO
    {
        public int Id { get; set; }
        public int EskaeraId { get; set; }
        public int? ErreserbaId { get; set; }
        public string PdfIzena { get; set; }
        public DateTime Data { get; set; }
        public decimal Totala { get; set; }
    }
}
