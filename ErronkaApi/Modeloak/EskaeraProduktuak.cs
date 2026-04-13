using ErronkaApi.Modeloak;

namespace ErronkaApi.Modeloak
{
    public class EskaeraProduktuak
    {
        public virtual int Id { get; set; }

        public virtual Eskaera Eskaera { get; set; }

        public virtual Produktua Produktua { get; set; }

        public virtual int Kantitatea { get; set; }
        public virtual string Egoera { get; set; }

        public virtual decimal PrezioUnitarioa { get; set; }

        public virtual decimal Guztira { get; set; }
    }
}
