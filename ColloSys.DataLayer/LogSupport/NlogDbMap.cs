#region references
using ColloSys.DataLayer.Infra.Domain;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
#endregion

namespace ColloSys.DataLayer.Infra.Mapping
{
    public class NlogDbMap : ClassMapping<NlogDb>
    {
        public NlogDbMap()
        {
            Table("G_LOGGING");

            #region property
            Id(x => x.nlogID, map => map.Generator(Generators.Identity));
            Property(x => x.DateTime);
            Property(x => x.Logger);
            Property(x => x.UserName);
            Property(x => x.LogLevel);
            Property(x => x.Message, map=> map.Length(4001));
            #endregion
        }                                             
        //int.MaxValue
    }                                                 
}
