namespace ErronkaApi.Modeloak
{
    public class Faktura
    {
        public virtual int id { get; set; }
        public virtual int eskaeraId { get; set; }
        public virtual string pdfIzena { get; set; }
        public virtual DateTime data { get; set; }
        public virtual decimal? guztira { get; set; }
    }
}
