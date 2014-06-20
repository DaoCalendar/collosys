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
using ColloSys.DataLayer.SessionMgr;
using NHibernate.Transform;

#endregion


namespace ColloSys.FileUploaderService.DbLayer
{
    public class DbLayer : IDbLayer
    {
        public FileScheduler GetNextFileForSchedule()
        {
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    FileScheduler fs = null;
                    FileDetail fd = null;
                    var enddatetime = DateTime.Now.AddDays(-40);
                    var obj = session.QueryOver(() => fs)
                                     .JoinAlias(() => fs.FileDetail, () => fd)
                                     .Where(c => (c.UploadStatus == ColloSysEnums.UploadStatus.UploadRequest
                                                  || c.UploadStatus == ColloSysEnums.UploadStatus.RetryUpload))
                                     .And(c => c.IsImmediate || c.StartDateTime <= DateTime.Now)
                                     .And(c => c.CreatedOn > enddatetime)
                                     .Fetch(x => x.FileDetail).Eager
                                     .Fetch(x => x.FileStatuss).Eager
                                     .Fetch(x => x.FileDetail.FileColumns).Eager
                                     .TransformUsing(Transformers.DistinctRootEntity)
                                     .OrderBy(x => x.FileDate).Asc
                                     .ThenBy(x => x.CreatedOn).Asc
                                     .List();

                    
                    FileScheduler file2Upload = null;
                    IList<FileScheduler> filesWaiting = new List<FileScheduler>();
                    foreach (var fileScheduler in obj)
                    {
                        fileScheduler.FileDetail.FileMappings =
                            session.QueryOver<FileMapping>()
                                .Where(x => x.FileDetail.Id == fileScheduler.FileDetail.Id)
                                .List();
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
                        var fileDetailOnAlias =
                            session.QueryOver<FileDetail>()
                            .Where(x => x.AliasName == dependAlias)
                            .SingleOrDefault();

                        var dependFileScheduler = session.QueryOver<FileScheduler>()
                            .Where(x => x.FileDate == schedulerDate)
                            .And(x=>x.FileDetail.Id==fileDetailOnAlias.Id)
                            .And(
                                x =>
                                    x.UploadStatus == ColloSysEnums.UploadStatus.Done ||
                                    x.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                            .List();
                        //var dependFileScheduler = session.QueryOver(() => fs1)
                        //                                 .Fetch(x => x.FileDetail).Eager
                        //                                 .JoinAlias(() => fs1.FileDetail, () => fd1)
                        //                                 .Where(c => c.FileDate == schedulerDate && fd1.AliasName == dependAlias)
                        //                                 .And(c => c.UploadStatus == ColloSysEnums.UploadStatus.Done ||
                        //                                     c.UploadStatus == ColloSysEnums.UploadStatus.DoneWithError)
                        //                                     .TransformUsing(Transformers.DistinctRootEntity)
                        //                                 .List<FileScheduler>();

                        if ((dependFileScheduler.Count != 0)
                            && (dependFileScheduler.Count == fileDetailOnAlias.FileCount))
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

        public IList<string> GetValuesFromKey(ColloSysEnums.Activities area, string key)
        {
            using (var session = SessionManager.GetStatelessSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var data = session.QueryOver<GKeyValue>()
                                      .Where(x => x.Area == area && x.ParamName == key)
                                      .Select(x => x.Value)
                                      .List<string>();
                    tx.Rollback();
                    return data;
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

        public IList<TEntity> GetDataForPreviousDay<TEntity>(ColloSysEnums.FileAliasName aliasName, DateTime date, uint filecount) where TEntity : Entity, IFileUploadable
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

        public TEntity GetRecordForUpdate<TEntity>(string accountNo)
            where TEntity : Entity, IDelinquentCustomer
        {
            TEntity objEntity = null;
            using (var session = SessionManager.GetNewSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    objEntity = session.QueryOver<TEntity>()
                        .Where(x => x.AccountNo == accountNo).SingleOrDefault();
                    tx.Commit();
                }
            }
            
            return objEntity;
        }
    }
}
