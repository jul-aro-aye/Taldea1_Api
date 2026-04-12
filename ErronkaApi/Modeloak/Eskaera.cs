using ErronkaApi.Modeloak;

namespace ErronkaApi.Modeloak
{
    public class Eskaera
    {
        public virtual int id { get; set; }

        public virtual int? mahaia_id { get; set; }
        public virtual int erabiltzaileId { get; set; }
        public virtual int komensalak { get; set; }
        public virtual string egoera { get; set; }
        public virtual string sukaldeaEgoera { get; set; }
        public virtual DateTime sortzeData { get; set; }
        public virtual DateTime? itxieraData { get; set; }
        public virtual int? erreserbaId { get; set; }

        public virtual IList<EskaeraMahaiak> EskaeraMahaiak { get; set; } = new List<EskaeraMahaiak>();

        public virtual IList<EskaeraProduktuak> EskaeraProduktuak { get; set; } = new List<EskaeraProduktuak>();
    }
}
