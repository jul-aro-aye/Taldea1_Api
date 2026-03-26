using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

namespace ErronkaApi.Mapeoak
{
    public class MahaiaMap : ClassMap<Mahaia>
    {
        public MahaiaMap()
        {
            Table("mahaiak");

            Id(x => x.id).Column("id").GeneratedBy.Identity();
            Map(x => x.zenbakia).Column("zenbakia");
            Map(x => x.kapazitatea).Column("kapazitatea");
            Map(x => x.egoera).Column("egoera");

            HasMany(x => x.EskaeraMahaiak)
                .KeyColumn("mahaia_id")
                .Cascade.All()
                .Inverse();
        }
    }
}
