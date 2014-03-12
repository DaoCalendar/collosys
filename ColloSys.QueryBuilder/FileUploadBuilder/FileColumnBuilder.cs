#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

#endregion



namespace ColloSys.QueryBuilder.FileUploadBuilder
{
    public class FileColumnBuilder : QueryBuilder<FileColumn>
    {
        public override QueryOver<FileColumn, FileColumn> DefaultQuery()
        {
            return QueryOver.Of<FileColumn>();
        }

        [Transaction]
        public IEnumerable<FileColumn> OnFileDetailId(Guid fileDetailId)
        {
            return
                SessionManager.GetCurrentSession()
                              .QueryOver<FileColumn>()
                              .Where(c => c.FileDetail.Id == fileDetailId)
                              .List();
        }
    }

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

    public class FileMappingBuilder : QueryBuilder<FileMapping>
    {
        public override QueryOver<FileMapping, FileMapping> DefaultQuery()
        {
            return QueryOver.Of<FileMapping>();
        }
    }

    public class FileSchedulerBuilder : QueryBuilder<FileScheduler>
    {
        public override QueryOver<FileScheduler, FileScheduler> DefaultQuery()
        {
            return QueryOver.Of<FileScheduler>();
        }

        [Transaction]
        public IEnumerable<FileScheduler> CacsForAllocation()
        {
            return SessionManager.GetCurrentSession().QueryOver<FileScheduler>()
                               .Where(x => x.ScbSystems == ScbEnums.ScbSystems.CACS)
                               .And(x => x.Category == ScbEnums.Category.Activity)
                               .And(x => x.AllocBillDone == false)
                               .Fetch(x => x.FileDetail).Eager
                               .List();
        }

        [Transaction]
        public IEnumerable<FileScheduler> OnMaxBackDate(DateTime maxBackdate)
        {
           return SessionManager.GetCurrentSession().QueryOver<FileScheduler>()
                          .Where(x => x.CreatedOn > maxBackdate)
                          .OrderBy(c => c.StartDateTime).Desc
                          .Fetch(c => c.FileStatuss).Eager
                          .Fetch(c => c.FileDetail).Eager
                          .TransformUsing(Transformers.DistinctRootEntity)
                          .List();
        }

        [Transaction]
        public IEnumerable<FileScheduler> DateRange(DateTime fromDate, DateTime toDate)
        {
           return SessionManager.GetCurrentSession().QueryOver<FileScheduler>()
                          .Where(x => x.FileDate >= fromDate && x.FileDate <= toDate)
                          .OrderBy(c => c.StartDateTime).Desc
                          .Fetch(c => c.FileStatuss).Eager
                          .Fetch(c => c.FileDetail).Eager
                          .TransformUsing(Transformers.DistinctRootEntity)
                          .List();
        }

        [Transaction]
        public IEnumerable<FileScheduler> OnFileSchedulers(IEnumerable<FileScheduler> fileScheduler)
        {
            return SessionManager.GetCurrentSession().QueryOver<FileScheduler>()
                                 .Fetch(x => x.FileDetail).Eager
                                 .WhereRestrictionOn(x => x.Id).IsIn(fileScheduler.Select(x => x.Id).ToArray())
                                 .List();
        }

        [Transaction]
        public IEnumerable<FileScheduler> OnSystemCategoryFileDate(ScbEnums.ScbSystems systems,ScbEnums.Category category, DateTime fileDate)
        {
           return SessionManager.GetCurrentSession().QueryOver<FileScheduler>()
                                      .Where(x => x.ScbSystems == systems)
                                      .And(x => x.Category == category)
                                      .And(x => x.FileDate == fileDate)
                                      .Fetch(x => x.FileDetail).Eager
                                      .Skip(0).Take(500).List();
        }

        [Transaction]
        public int Count(ColloSysEnums.FileAliasName alias,ulong fileSize, DateTime fileDate,string fileName)
        {
          return  SessionManager.GetCurrentSession().Query<FileScheduler>()
                                      .Count(x => x.FileDetail.AliasName == alias
                                                  && x.FileSize == fileSize
                                                  && x.FileDate >= fileDate
                                                  && x.FileName.Substring(16).Equals(fileName)
                                                  && x.UploadStatus != ColloSysEnums.UploadStatus.Error);
        }
    }

    public class FileStatusBuilder : QueryBuilder<FileStatus>
    {
        public override QueryOver<FileStatus, FileStatus> DefaultQuery()
        {
            return QueryOver.Of<FileStatus>();
        }
    }

    public class FileValueMappingBuilder : QueryBuilder<FileValueMapping>
    {
        public override QueryOver<FileValueMapping, FileValueMapping> DefaultQuery()
        {
            return QueryOver.Of<FileValueMapping>();
        }
    }
}

