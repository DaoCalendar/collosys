using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ColloSys.DataLayer.Generic
{
    class VersionInfoMap : ClassMapping<VersionInfo>
    {
        public VersionInfoMap()
        {
            Id(x => x.Version, map => { map.Generator(Generators.Assigned); map.UnsavedValue(0); map.Column("Version"); });
            Property(x => x.AppliedOn, map => map.Type(NHibernateUtil.DateTime2));
        }
    }
}
