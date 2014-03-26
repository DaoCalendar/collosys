#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.SharedDomain;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Fluent;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    internal static class AudtingSetup
    {
        public static void Setup(Configuration conf)
        {
            // get config instance
            var nhconfig = new FluentConfiguration();
            nhconfig.SetRevisionEntity<EnversAuditInfo>(x => x.Id, x => x.RevisionDate);

            IList<Type> types = Assembly.GetExecutingAssembly().GetExportedTypes();
            IList<Type> auditType = new List<Type>();
            IList<Type> baseTypes = new List<Type>
                {
                    typeof(Entity), typeof(Allocations), 
                    typeof(CustomerInfo), typeof(Payment),
                    typeof(UploadableEntity)
                };
            foreach (var type in types.Where( x=> x.IsClass && !x.IsAbstract))
            {
                if(!baseTypes.Contains(type.BaseType)) continue;
                auditType.Add(type);
            }
            nhconfig.Audit(auditType);

            conf.SetEnversProperty(ConfigurationKey.AuditTablePrefix, "ZHISTORY_");
            conf.SetEnversProperty(ConfigurationKey.AuditTableSuffix, string.Empty);
            conf.SetEnversProperty(ConfigurationKey.DefaultSchema, "dbo");
            conf.SetEnversProperty(ConfigurationKey.RevisionFieldName, "Revision");
            conf.SetEnversProperty(ConfigurationKey.RevisionTypeFieldName, "RevisionAction");
            conf.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
            conf.SetEnversProperty(ConfigurationKey.DoNotAuditOptimisticLockingField, false);

            // intergrate with nhibernate
            conf.IntegrateWithEnvers(nhconfig);
        }
    }
}