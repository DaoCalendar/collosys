#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Transform;

#endregion


namespace ColloSys.QueryBuilder.FileUploadBuilder
{
    public class FileDetailBuilder : QueryBuilder<FileDetail>
    {
        public override QueryOver<FileDetail, FileDetail> DefaultQuery()
        {
            return QueryOver.Of<FileDetail>();
        }

        [Transaction]
        public IEnumerable<FileDetail> ForROrE()
        {
            return SessionManager.GetCurrentSession().QueryOver<FileDetail>()
                                 .Where(c => c.ScbSystems == ScbEnums.ScbSystems.EBBS || c.ScbSystems == ScbEnums.ScbSystems.RLS)
                                 .List();
        }

        [Transaction]
        public FileDetail OnAliasName(ColloSysEnums.FileAliasName alias)
        {
            return SessionManager.GetCurrentSession().QueryOver<FileDetail>()
                                 .Where(x => x.AliasName == alias)
                                 .Fetch(x => x.FileColumns).Eager
                                 .TransformUsing(Transformers.DistinctRootEntity)
                                 .SingleOrDefault();
        }

        [Transaction]
        public IEnumerable<ColloSysEnums.FileAliasName> ForROrEAliasNames()
        {
            return SessionManager.GetCurrentSession().QueryOver<FileDetail>()
                                 .Where(x => x.ScbSystems == ScbEnums.ScbSystems.EBBS || x.ScbSystems == ScbEnums.ScbSystems.RLS)
                                 .Select(x => x.AliasName)
                                 .OrderBy(x => x.AliasName).Asc
                                 .List<ColloSysEnums.FileAliasName>();
        }

        [Transaction]
        public IEnumerable<FileDetail> OnSystemCategory(ScbEnums.ScbSystems systems,ScbEnums.Category category)
        {
            return SessionManager.GetCurrentSession().QueryOver<FileDetail>()
                                 .Where(x => x.ScbSystems == systems)
                                 .And(x => x.Category == category)
                                 .Skip(0).Take(500).List();
        }
    }
}