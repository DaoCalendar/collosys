#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploadService.Interfaces;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using NLog;

#endregion

namespace ColloSys.FileUploadService.Implementers
{
    public class DbLayer : IDBLayer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public FileScheduler GetNextFileForSchedule()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var enddatetime = DateTime.Now.AddDays(-40);
                    var obj = session.QueryOver<FileScheduler>()
                                     .Where(c => (c.UploadStatus == ColloSysEnums.UploadStatus.UploadRequest
                                                  || c.UploadStatus == ColloSysEnums.UploadStatus.RetryUpload))
                                     .And(c => c.IsImmediate || c.StartDateTime <= DateTime.Now)
                                     .And(c => c.CreatedOn > enddatetime)
                                     .Fetch(x => x.FileDetail).Eager
                                     .TransformUsing(Transformers.DistinctRootEntity)
                                     .OrderBy(x => x.FileDate).Asc
                                     .ThenBy(x => x.CreatedOn).Asc
                                     .List();

                    FileScheduler file2Upload = null;
                    IList<FileScheduler> filesWaiting = new List<FileScheduler>();
                    foreach (var fileScheduler in obj)
                    {
                        // if no depend alias then continue with upload
                        var dependAlias = fileScheduler.FileDetail.DependsOnAlias;
                        if (dependAlias == null)
                        {
                            file2Upload = fileScheduler;
                            break;
                        }

                        FileScheduler fs1 = null;
                        FileDetail fd1 = null;
                        var schedulerDate = fileScheduler.FileDate.Date;
                        var dependFileScheduler = session.QueryOver(() => fs1)
                                                         .Fetch(x => x.FileDetail).Eager
                                                         .JoinAlias(() => fs1.FileDetail, () => fd1)
                                                         .Where(c => c.FileDate == schedulerDate && fd1.AliasName == dependAlias)
                                                         .And(c => c.UploadStatus == ColloSysEnums.UploadStatus.Done ||
                                                             c.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                                                         .List<FileScheduler>();

                        if ((dependFileScheduler.Count != 0)
                            && (dependFileScheduler.Count == dependFileScheduler.First().FileDetail.FileCount))
                        {
                            file2Upload = fileScheduler;
                            break;
                        }

                        // if depend File not scheduled then set description to waiting.
                        var waitingDescription = "Waiting for all " + dependAlias.ToString();
                        // if description already set then continue without set other wise set description
                        if (fileScheduler.StatusDescription == waitingDescription) continue;
                        fileScheduler.StatusDescription = waitingDescription;
                        filesWaiting.Add(fileScheduler);
                    }

                    //eager load other stuff
                    if (file2Upload != null)
                    {
                        // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                        file2Upload.FileStatuss.Count();
                        file2Upload.FileDetail.FileColumns.Count();
                        file2Upload.FileDetail.FileMappings.Count();
                        foreach (var mapping in file2Upload.FileDetail.FileMappings)
                        {
                            mapping.FileValueMappings.Count();
                        }
                        // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                    }

                    // if there is any waiting files then set status description to waiting.
                    if (filesWaiting.Count > 0)
                    {
                        foreach (var fileScheduler in filesWaiting)
                        {
                            session.SaveOrUpdate(fileScheduler);
                        }

                        tx.Commit();
                    }
                    else
                    {
                        tx.Rollback();
                    }

                    return file2Upload;
                }
            }
        }

        public int GetDoneFile(ColloSysEnums.FileAliasName aliasName, DateTime date)
        {
            var date2 = date.Date;
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    FileScheduler fs = null;
                    FileDetail fd = null;
                    var list = session.QueryOver(() => fs)
                                      .JoinAlias(() => fs.FileDetail, () => fd)
                                      .Fetch(x => x.FileDetail).Eager
                                      .Where(x => x.FileDate == date2 && fd.AliasName == aliasName)
                                      .And(x => x.UploadStatus == ColloSysEnums.UploadStatus.Done ||
                                          x.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                                      .RowCount();

                    tx.Rollback();

                    return list;
                }
            }
        }

        public void CommitData<TEntity>(IList<TEntity> data, FileScheduler file, IRowCounter counter)
                where TEntity : Entity
        {

            if (data.Count == 0)
            {
                return;
            }

            _logger.Info(string.Format("ReadFile: updating filestatus table. " +
                           "rows uploaded {0}. valid {1}",
                           counter.GetTotalRecordCount(), counter.GetValidRecordCount()));

            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    foreach (var entity in data)
                    {
                        session.SaveOrUpdate(entity);
                    }

                    var status = new FileStatus
                    {
                        TotalRows = counter.GetTotalRecordCount(),
                        ValidRows = counter.GetValidRecordCount(),
                        UploadedRows = counter.GetUploadedRecordCount(),
                        DuplicateRows = counter.GetDuplicateRecordCount(),
                        IgnoredRows = counter.GetIgnoredRecordCount(),
                        ErrorRows = counter.GetErrorRecordCount(),
                        FileScheduler = file,
                        UploadStatus = ColloSysEnums.UploadStatus.ActInserting,
                        EntryDateTime = DateTime.Now
                    };
                    session.SaveOrUpdate(status);

                    tx.Commit();
                }
            }

        }

        public void SetDoneStatus(FileScheduler fileScheduler, IRowCounter counter)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    //var fx = session.Get<FileScheduler>(fileScheduler.Id);
                    session.Refresh(fileScheduler);

                    fileScheduler.ErrorRows = counter.GetErrorRecordCount();
                    fileScheduler.ValidRows = counter.GetUploadedRecordCount();
                    fileScheduler.TotalRows = counter.GetTotalRecordCount();
                    fileScheduler.EndDateTime = DateTime.Now;

                    if (fileScheduler.ValidRows <= 0)
                    {
                        fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.Error;
                        fileScheduler.StatusDescription = "0 Valid Rows";
                    }
                    else if (fileScheduler.ErrorRows > 0)
                    {
                        fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.DoneWithError;
                    }
                    else
                    {
                        fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.Done;
                    }

                    _logger.Info(string.Format("ReadFile: updating fileschduler-status table. " +
                                               "rows uploaded {0}. status {1}",
                                               fileScheduler.TotalRows,
                                               fileScheduler.UploadStatus));

                    fileScheduler.FileStatuss.Add(new FileStatus
                    {
                        EntryDateTime = DateTime.Now,
                        FileScheduler = fileScheduler,
                        TotalRows = counter.GetTotalRecordCount(),
                        ValidRows = counter.GetValidRecordCount(),
                        UploadedRows = counter.GetUploadedRecordCount(),
                        DuplicateRows = counter.GetDuplicateRecordCount(),
                        ErrorRows = counter.GetErrorRecordCount(),
                        IgnoredRows = counter.GetIgnoredRecordCount(),
                        UploadStatus = fileScheduler.UploadStatus,
                    });

                    session.SaveOrUpdate(fileScheduler);
                    tx.Commit();
                }
            }
        }

        public void ChangeStatus(FileScheduler fileScheduler)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    //session.Refresh(fileScheduler);

                    _logger.Info(string.Format("ReadFile: updating fileschduler-status table. " +
                                               "rows uploaded {0}. status {1}",
                                               fileScheduler.TotalRows,
                                               fileScheduler.UploadStatus));

                    fileScheduler.FileStatuss.Add(new FileStatus
                    {
                        EntryDateTime = DateTime.Now,
                        FileScheduler = fileScheduler,
                        TotalRows = fileScheduler.TotalRows,
                        ValidRows = fileScheduler.ValidRows,
                        UploadedRows = 0,
                        DuplicateRows = 0,
                        IgnoredRows = 0,
                        ErrorRows = 0,
                        UploadStatus = fileScheduler.UploadStatus
                    });

                    session.SaveOrUpdate(fileScheduler);
                    tx.Commit();
                }
            }
        }

        public List<TEntity> GetDataForDate<TEntity>(DateTime dateTime)
            where TEntity : Entity, IFileUploadable
        {
            using (var session = SessionManager.GetStatelessSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var data = session.QueryOver<TEntity>()
                                      .Where(x => x.FileDate == dateTime)
                                      .List();
                    tx.Rollback();
                    return new List<TEntity>(data);
                }
            }
        }

        public List<string> GetAccountNosForDate<TEntity>(DateTime dateTime)
           where TEntity : Entity, IUniqueKey
        {
            using (var session = SessionManager.GetStatelessSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var data = session.QueryOver<TEntity>()
                                      .Where(x => x.FileDate == dateTime)
                                      .Select(x => x.AccountNo)
                                      .List<string>();
                    tx.Rollback();
                    return new List<string>(data);
                }
            }
        }

        public IList<TEntity> GetTableData<TEntity>()
             where TEntity : Entity
        {
            using (var session = SessionManager.GetStatelessSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var data = session.QueryOver<TEntity>()
                                      .List();
                    tx.Rollback();
                    return data;
                }
            }
        }

        public IList<TEntity> GetDataForPreviousDay<TEntity>(ColloSysEnums.FileAliasName aliasName,
        DateTime date, uint filecount)
            where TEntity : Entity, IFileUploadable
        {
            using (var session = SessionManager.GetStatelessSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    FileScheduler fs = null;
                    FileDetail fd = null;
                    var date2 = date.Date;
                    var lastuploadedfile = session.QueryOver(() => fs)
                                               .JoinAlias(x => x.FileDetail, () => fd)
                                               .Where(() => fd.AliasName == aliasName)
                                               .And(x => x.FileDate < date2)
                                               .And(x => x.UploadStatus == ColloSysEnums.UploadStatus.Done ||
                                                         x.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                                               .OrderBy(x => x.FileDate).Desc
                                               .Take((int)filecount)
                                               .List<FileScheduler>();

                    if (lastuploadedfile.Count <= 0)
                    {
                        return new List<TEntity>();
                    }
                    var maxdate = lastuploadedfile.Max(x => x.FileDate);
                    var lastuploadedfileid = lastuploadedfile.Where(x => x.FileDate == maxdate).Select(x => x.Id);

                    var data = session.QueryOver<TEntity>()
                                      .WhereRestrictionOn(x => x.FileScheduler.Id)
                                      .IsIn(lastuploadedfileid.ToArray())
                                      .List();

                    tx.Rollback();
                    return data;
                }
            }
        }

        public IList<string> GetValuesFromKey(ColloSysEnums.Activities area, string key)
        {
            using (var session = SessionManager.GetStatelessSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var data = session.QueryOver<GKeyValue>()
                                      .Where(x => x.Area == area && x.Key == key)
                                      .Select(x => x.Value)
                                      .List<string>();
                    tx.Rollback();
                    return data;
                }
            }
        }

        public bool SaveOrUpdateData<TEntity>(IEnumerable<TEntity> data)
            where TEntity : Entity
        {
            var dataList = data as IList<TEntity> ?? data.ToList();
            for (var i = 0; i < dataList.Count(); i += 1000)
            {

                var batchToSave = dataList.Skip(i).Take(1000);

                using (var session = SessionManager.GetNewSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        foreach (var entity in batchToSave)
                        {
                            session.SaveOrUpdate(entity);
                        }

                        tx.Commit();
                    }
                }
            }

            return true;
        }

        public bool MergeData<TEntity>(IEnumerable<TEntity> data)
           where TEntity : Entity, IFileUploadable
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    foreach (var entity in data)
                    {
                        session.Merge(entity);
                    }

                    tx.Commit();
                    return true;
                }
            }
        }

        public void ResetFileStatus()
        {
            try
            {
                using (var session = SessionManager.GetNewSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        var fileSchedulerList = session.QueryOver<FileScheduler>()
                                                       .Where(c => c.StartDateTime <= DateTime.Now)
                                                       .And(c => (c.UploadStatus != ColloSysEnums.UploadStatus.Done) &&
                                                           (c.UploadStatus != ColloSysEnums.UploadStatus.DoneWithError) &&
                                                           (c.UploadStatus != ColloSysEnums.UploadStatus.Error) &&
                                                           (c.UploadStatus != ColloSysEnums.UploadStatus.UploadRequest))
                                                       .List();

                        foreach (var fileScheduler in fileSchedulerList)
                        {
                            fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.UploadRequest;
                            session.SaveOrUpdate(fileScheduler);
                        }

                        _logger.Fatal(string.Format("DB Layer - ResetStatus: Resetting status of {0} files.",
                                                  fileSchedulerList.Count));
                        tx.Commit();
                    }
                }
            }
            catch (HibernateException e)
            {
                _logger.Error("ChangeStatus Exception : " + e);
            }
        }

        public Dictionary<string, Decimal> GetUnbilledAmounts(DateTime date)
        {
            var unbilledAmounts = new Dictionary<string, decimal>();
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.QueryOver<CUnbilled>()
                           .Where(x => x.FileDate == date)
                           .SelectList(list => list.SelectGroup(m => m.AccountNo).SelectSum(m => m.UnbilledAmount))
                           .List<object[]>()
                           .ForEach(props => unbilledAmounts.Add((string)props[0], (decimal)props[1]));

                    tx.Rollback();
                }
            }

            return unbilledAmounts;
        }

        public Dictionary<string, Decimal> GetCustTotalDue(DateTime date)
        {
            var dueAmount = new Dictionary<string, decimal>();
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.QueryOver<CLiner>()
                           .Where(x => x.FileDate == date)
                           .SelectList(
                               list => list.SelectGroup(m => m.GlobalCustId).SelectSum(m => m.OutStandingBalance))
                           .List<object[]>()
                           .ForEach(props => dueAmount.Add((string)props[0], (decimal)props[1]));

                    tx.Rollback();
                }
            }

            return dueAmount;
        }

        public ISet<string> GetZGlobalCustId(DateTime date)
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var result = session.QueryOver<CLiner>()
                                        .Where(x => x.FileDate == date && x.Flag == ColloSysEnums.DelqFlag.Z)
                                        .Select(x => x.GlobalCustId).List<string>();

                    tx.Rollback();
                    return new HashSet<string>(result);
                }
            }
        }

    }
}