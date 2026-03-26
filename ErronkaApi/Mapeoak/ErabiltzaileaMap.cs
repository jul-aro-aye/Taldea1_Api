using ErronkaApi.Modeloak;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErronkaApi.Mapeoak
{
    
    public class ErabiltzaileaMap : ClassMap<Erabiltzailea>
    {
        /// <summary>
        /// Erabiltzailea entitatearen mapeoaren klasea, Fluent NHibernate erabiliz.
        /// </summary>
        /// <param name="id">
        /// gure erabiltzailea entitatearen id atributua, datu baseko "id" zutabearekin mapatuta dagoena, eta auto-increment bidez sortzen dena.
        /// </param>
        /// <remarks>
        /// references(x => x.rola).Column("rola_id").Not.Nullable().Not.LazyLoad(); lerroak erabiltzailea entitatearen rola atributua datu baseko "rola_id" zutabearekin mapatzen du, eta ez du uzten null balioak onartzen eta ez du lazy loading-a erabiltzen.
        /// </remarks>
        public ErabiltzaileaMap()
        { 
            Table("erabiltzaileak");
            Id(x => x.id).Column("id").GeneratedBy.Identity();
            Map(x => x.erabiltzailea).Column("erabiltzailea").Length(45);
            Map(x => x.emaila).Column("email").Length(100);
            Map(x => x.pasahitza).Column("pasahitza").Length(45);
            References(x => x.rola).Column("rola_id").Not.Nullable().Not.LazyLoad();
            Map(x => x.ezabatua).Column("ezabatua");
            Map(x => x.txat).Column("txat").Not.Nullable();
        }
    }
}
