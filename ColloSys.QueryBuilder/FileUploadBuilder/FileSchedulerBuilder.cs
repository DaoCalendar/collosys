#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.QueryBuilder.BaseTypes;
using ColloSys.QueryBuilder.TransAttributes;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

#endregion

namespace ColloSys.QueryBuilder.FileUploadBuilder
{
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
}