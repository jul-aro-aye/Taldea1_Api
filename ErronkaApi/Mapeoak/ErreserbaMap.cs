using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

namespace ErronkaApi.Mapeoak
{
    public class ErreserbaMap : ClassMap<Erreserba>
    {
        public ErreserbaMap()
        {
            Table("erreserbak");

            Id(x => x.id).Column("id").GeneratedBy.Identity();
            Map(x => x.mahaiaId).Column("mahaia_id");
            Map(x => x.bezeroaIzena).Column("bezeroa_izena");
            Map(x => x.telefonoa).Column("telefonoa");
            Map(x => x.data).Column("data");
            Map(x => x.erreserbaData).Column("erreserba_data");
            Map(x => x.txanda).Column("txanda");
            Map(x => x.pertsonaKopurua).Column("pertsona_kopurua");
            Map(x => x.egoera).Column("egoera");
        }
    }
}
