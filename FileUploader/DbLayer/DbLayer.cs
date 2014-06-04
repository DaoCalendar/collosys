﻿#region references

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
using ColloSys.FileUploader.DbLayer;
using NHibernate.Transform;

#endregion


namespace ColloSys.FileUploaderService.v2.DbLayer
{
  public  class DbLayer: IDbLayer
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
                                     .Fetch(x => x.FileDetail.FileMappings).Eager
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
                                      .Where(x => x.Area == area && x.Key == key)
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
    }
}
