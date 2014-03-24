#region references

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class FileSchedulerBuilder : Repository<FileScheduler>
    {
        public override QueryOver<FileScheduler, FileScheduler> WithRelation()
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
        public IEnumerable<FileScheduler> OnSystemCategoryFileDate(ScbEnums.ScbSystems systems, ScbEnums.Category category, DateTime fileDate)
        {
            return SessionManager.GetCurrentSession().QueryOver<FileScheduler>()
                                 .Fetch(x => x.FileDetail).Eager
                                 .Where(x => x.ScbSystems == systems)
                                 .And(x => x.Category == category)
                                 .And(x => x.FileDate == fileDate)
                                 .Skip(0).Take(500).List();
        }

        [Transaction]
        public int Count(ColloSysEnums.FileAliasName alias, ulong fileSize, DateTime fileDate, string fileName)
        {
            return SessionManager.GetCurrentSession().Query<FileScheduler>()
                                  .Count(x => x.FileDetail.AliasName == alias
                                              && x.FileSize == fileSize
                                              && x.FileDate >= fileDate
                                              && x.FileName.Substring(16).Equals(fileName)
                                              && x.UploadStatus != ColloSysEnums.UploadStatus.Error);
        }

        [Transaction]
        public IEnumerable<FileScheduler> NextFileForSchedule()
        {
            FileScheduler fs = null;
            FileDetail fd = null;
            var enddatetime = DateTime.Now.AddDays(-40);
            return SessionManager.GetCurrentSession().QueryOver(() => fs)
                             .JoinAlias(() => fs.FileDetail, () => fd)
                             .Where(c => (c.UploadStatus == ColloSysEnums.UploadStatus.UploadRequest
                                          || c.UploadStatus == ColloSysEnums.UploadStatus.RetryUpload))
                             .And(c => c.IsImmediate || c.StartDateTime <= DateTime.Now)
                             .And(c => c.CreatedOn > enddatetime)
                             .Fetch(x => x.FileDetail).Eager
                             .Fetch(x => x.FileStatuss).Eager
                             .Fetch(x => x.FileDetail.FileColumns).Eager
                             .Fetch(x => x.FileDetail.FileMappings).Eager
                             .TransformUsing(Transformers.DistinctRootEntity)
                             .OrderBy(x => x.FileDate).Asc
                             .ThenBy(x => x.CreatedOn).Asc
                             .List();
        }

        [Transaction]
        public IEnumerable<FileScheduler> DependOnAliasAndSheduleDate(DateTime schedulerDate, ColloSysEnums.FileAliasName? dependAlias)
        {
            FileScheduler fs1 = null;
            FileDetail fd1 = null;
            return SessionManager.GetCurrentSession().QueryOver(() => fs1)
                                 .Fetch(x => x.FileDetail).Eager
                                 .JoinAlias(() => fs1.FileDetail, () => fd1)
                                 .Where(c => c.FileDate == schedulerDate && fd1.AliasName == dependAlias)
                                 .And(c => c.UploadStatus == ColloSysEnums.UploadStatus.Done ||
                                           c.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                                 .List<FileScheduler>();
        }

        [Transaction]
        public int DoneFileCount(DateTime fileDate, ColloSysEnums.FileAliasName aliasName)
        {
            FileScheduler fs = null;
            FileDetail fd = null;
            return SessionManager.GetCurrentSession().QueryOver(() => fs)
                              .JoinAlias(() => fs.FileDetail, () => fd)
                              .Fetch(x => x.FileDetail).Eager
                              .Where(x => x.FileDate == fileDate && fd.AliasName == aliasName)
                              .And(x => x.UploadStatus == ColloSysEnums.UploadStatus.Done ||
                                  x.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                              .RowCount();
        }

        [Transaction]
        public IEnumerable<FileScheduler> ResetFileStatus()
        {
            return SessionManager.GetCurrentSession().QueryOver<FileScheduler>()
                                 .Where(c => c.StartDateTime <= DateTime.Now)
                                 .And(c => (c.UploadStatus != ColloSysEnums.UploadStatus.Done) &&
                                           (c.UploadStatus != ColloSysEnums.UploadStatus.DoneWithError) &&
                                           (c.UploadStatus != ColloSysEnums.UploadStatus.Error) &&
                                           (c.UploadStatus != ColloSysEnums.UploadStatus.UploadRequest))
                                 .List();
        }

        [Transaction]
        public IEnumerable<FileScheduler> LastUploadedFiles(ColloSysEnums.FileAliasName aliasName, 
            DateTime fileDate, uint filecount = 10)
        {
            FileScheduler fs = null;
            FileDetail fd = null;
            var date2 = fileDate.Date;
            return SessionManager.GetCurrentSession().QueryOver(() => fs)
                                       .JoinAlias(x => x.FileDetail, () => fd)
                                       .Where(() => fd.AliasName == aliasName)
                                       .And(x => x.FileDate < date2)
                                       .And(x => x.UploadStatus == ColloSysEnums.UploadStatus.Done ||
                                                 x.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                                       .OrderBy(x => x.FileDate).Desc
                                       .Take((int)filecount)
                                       .List<FileScheduler>();
        }

    }
}