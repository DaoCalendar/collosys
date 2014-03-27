#region References

using System;
using System.Linq;
using System.Reflection;
using ColloSys.DataLayer.Allocation;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    internal static class MappingSetup
    {
        public static void Setup(Configuration config)
        {
            // create new model mapper
            var mapper = new ModelMapper();

            // add conventions
            FieldMappingConvention(mapper);
            PrimaryKeyNamingConvention(mapper);
            RelationshipMappingConventions(mapper);

            // add all mappings in executing assembly
            mapper.AddMappings(Assembly.GetAssembly(typeof(AllocPolicy)).GetTypes());

            // convert them to hbm
            var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            // add mappings to current configuration
            config.AddMapping(domainMapping);
        }

        private static void PrimaryKeyNamingConvention(ModelMapper mapper)
        {
            // primary key convention
            mapper.BeforeMapClass += (mi, t, map) => map.Id(x => x.Column(t.Name + "Id"));
        }

        private static void RelationshipMappingConventions(ModelMapper mapper)
        {
            // foreign key convention (many2one side)
            mapper.BeforeMapManyToMany += (insp, prop, map) =>
                                          map.Column(prop.LocalMember.GetPropertyOrFieldType().Name + "Id");
            mapper.BeforeMapManyToMany += (insp, prop, map) => map.Lazy(LazyRelation.NoLazy);

            // foreign key convention (many2one side)
            mapper.BeforeMapManyToOne += (insp, prop, map) =>
                                         map.Column(prop.LocalMember.GetPropertyOrFieldType().Name + "Id");
            mapper.BeforeMapManyToOne += (insp, prop, map) => map.Cascade(Cascade.None);
            mapper.BeforeMapManyToOne += (insp, prop, map) => map.Lazy(LazyRelation.NoLazy);

            // bag conventions (one2many side)
            mapper.BeforeMapBag += (insp, prop, map) =>
                                   map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));
            mapper.BeforeMapBag += (insp, prop, map) => map.Cascade(Cascade.DeleteOrphans | Cascade.All);
            mapper.BeforeMapBag += (insp, prop, map) => map.BatchSize(100);
            mapper.BeforeMapBag += (insp, prop, map) => map.Inverse(true);
            mapper.BeforeMapBag += (insp, prop, map) => map.Lazy(CollectionLazy.NoLazy);

            // set conventions (one2many side)
            mapper.BeforeMapSet +=
                (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));
            mapper.BeforeMapSet += (insp, prop, map) => map.Cascade(Cascade.DeleteOrphans | Cascade.All);
            mapper.BeforeMapSet += (insp, prop, map) => map.BatchSize(100);
            mapper.BeforeMapSet += (insp, prop, map) => map.Inverse(true);
            mapper.BeforeMapSet += (insp, prop, map) => map.Lazy(CollectionLazy.NoLazy);

            // list conventions (one2many side)
            mapper.BeforeMapList +=
                (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));
            mapper.BeforeMapList += (insp, prop, map) => map.Cascade(Cascade.DeleteOrphans | Cascade.All);
            mapper.BeforeMapList += (insp, prop, map) => map.BatchSize(100);
            mapper.BeforeMapList += (insp, prop, map) => map.Inverse(true);
            mapper.BeforeMapList += (insp, prop, map) => map.Lazy(CollectionLazy.NoLazy);
        }

        private static void FieldMappingConvention(ModelMapper mapper)
        {
            // string or enum field length
            mapper.BeforeMapProperty += (insp, prop, map) =>
                {
                    // make global required/not-requred
                    var propertyType = prop.LocalMember.GetPropertyOrFieldType();
                    var propertyName = prop.LocalMember.Name;
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = Nullable.GetUnderlyingType(propertyType);

                        // if value type - then set mandatory on basis of nullable-type
                        if ((propertyType.IsValueType) || (propertyType == typeof(string)))
                        //if (propertyType.IsValueType)
                        {
                            map.NotNullable(false);
                        }
                    }
                    // if value type - then set mandatory on basis of nullable-type
                    else if ((propertyType.IsValueType) || (propertyType == typeof(string)))
                    //else if (propertyType.IsValueType)
                    {
                        map.NotNullable(true);
                    }

                    // store enum as string
                    if (propertyType.IsEnum)
                    {
                        map.Type(typeof(EnumStringType<>).MakeGenericType(propertyType), null);
                        map.Length(50);
                    }

                    // store date/datetime with full precision
                    if (propertyType == typeof(DateTime))
                    {
                        string[] list = { "CreatedOn", "ApprovedOn" };
                        if (list.Any(x => x == propertyName) || (propertyName.ToUpperInvariant().EndsWith("DATETIME")))
                        {
                            map.Type(NHibernateUtil.DateTime2);
                        }
                        else if (propertyName.ToUpperInvariant().EndsWith("DATE"))
                        {
                            map.Type(NHibernateUtil.Date);
                        }
                    }
                };
        }
    }
}



//mapper.BeforeMapManyToOne += (insp, prop, map) => map.Fetch(FetchKind.Select);
//mapper.BeforeMapManyToOne += (insp, prop, map) => map.Lazy(LazyRelation.Proxy);
// add all mappings in executing assembly
// ********** add whole assembly
//mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
// ********** add by NameSpace
//mapper.AddMappings(Assembly.GetAssembly(typeof (AllocPolicy)).GetTypes());
//.Where(t => t.Namespace != null && t.Namespace.Equals("ColloSys.DataLayer.Domain")));
// ********** add by Class
//mapper.AddMapping(typeof(NhCustomerTest.CustomerMap));
//mapper.CompileMappingFor(new[] { typeof(NhCustomerTest.Customer) });
