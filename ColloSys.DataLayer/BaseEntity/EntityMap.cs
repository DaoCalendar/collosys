#region references

using System;
using System.Globalization;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

#endregion

namespace ColloSys.DataLayer.BaseEntity
{
    public abstract class EntityMap<T> : ClassMapping<T>
        where T : Entity
    {
        protected EntityMap()
        {
            Id(x => x.Id, map =>
                {
                    map.Generator(Generators.GuidComb);
                    map.UnsavedValue(Guid.Empty);
                });

            Version(x => x.Version, map =>
            {
                map.Generated(VersionGeneration.Never);
                map.UnsavedValue(default(int).ToString(CultureInfo.InvariantCulture));
            });

            Property(x => x.CreatedBy);

            Property(x => x.CreatedOn);

            Property(x => x.CreateAction);

            Cache(x => x.Usage(CacheUsage.ReadWrite));
        }
    }
}
