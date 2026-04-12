using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;

namespace ErronkaApi.Mapeoak
{
    public class FakturaMap : ClassMap<Faktura>
    {
        public FakturaMap()
        {
            Table("fakturak");

            Id(x => x.id).Column("id").GeneratedBy.Identity();
            Map(x => x.eskaeraId).Column("eskaera_id");
            Map(x => x.pdfIzena).Column("pdf_izena");
            Map(x => x.data).Column("data");
            Map(x => x.guztira).Column("guztira").Nullable();
        }
    }
}
