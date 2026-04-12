namespace ErronkaApi.Modeloak
{
    public class Erreserba
    {
        public virtual int id { get; set; }
        public virtual int mahaiaId { get; set; }
        public virtual string bezeroaIzena { get; set; }
        public virtual string? telefonoa { get; set; }
        public virtual DateTime data { get; set; }
        public virtual DateTime erreserbaData { get; set; }
        public virtual string txanda { get; set; }
        public virtual int pertsonaKopurua { get; set; }
        public virtual string egoera { get; set; }
    }
}
