using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

namespace ErronkaApi.Mapeoak
{
    public class EskaeraMap : ClassMap<Eskaera>
    {
        public EskaeraMap()
        {
            Table("eskaerak");

            Id(x => x.id).Column("id").GeneratedBy.Identity();
            Map(x => x.mahaia_id).Column("mahaia_id");
            Map(x => x.erabiltzaileId).Column("erabiltzaile_id");
            Map(x => x.komensalak).Column("komensalak");
            Map(x => x.egoera).Column("egoera");
            Map(x => x.sukaldeaEgoera).Column("sukaldea_egoera");
            Map(x => x.sortzeData).Column("sortze_data");
            Map(x => x.itxieraData).Column("itxiera_data");
            Map(x => x.erreserbaId).Column("erreserba_id").Nullable();

            HasMany(x => x.EskaeraMahaiak)
                .KeyColumn("eskaera_id")
                .Cascade.All()
                .Inverse();
            HasMany(x => x.EskaeraProduktuak)
                .KeyColumn("eskaera_id")
                .Cascade.All()
                .Inverse();
        }
    }
}
