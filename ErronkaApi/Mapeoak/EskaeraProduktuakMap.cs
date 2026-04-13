using FluentNHibernate.Mapping;
using ErronkaApi.Modeloak;

namespace ErronkaApi.Mapeoak
{
    public class EskaeraProduktuakMap : ClassMap<EskaeraProduktuak>
    {
        public EskaeraProduktuakMap()
        {
            Table("eskaera_produktuak");

            Id(x => x.Id).GeneratedBy.Identity();

            References(x => x.Eskaera)
                .Column("eskaera_id")
                .Not.Nullable();

            References(x => x.Produktua)
                .Column("produktua_id")
                .Not.Nullable();

            Map(x => x.Kantitatea)
                .Column("kantitatea");

            Map(x => x.Egoera)
                .Column("egoera");

            Map(x => x.PrezioUnitarioa)
                .Column("prezio_unitarioa");
        }
    }
}