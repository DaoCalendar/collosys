#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.QueryBuilder.ClientDataBuilder;
using ColloSys.QueryBuilder.FileUploadBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using NHibernate;
using NLog;

#endregion

namespace ColloSys.FileUploadService.Implementers
{
    public class DbLayer : IDBLayer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly FileSchedulerBuilder FileSchedulerBuilder = new FileSchedulerBuilder();
        private static readonly FileValueMappingBuilder FileValueMappingBuilder = new FileValueMappingBuilder();
        private static readonly CUnbilledBuilder CUnbilledBuilder = new CUnbilledBuilder();
        private static readonly CLinerBuilder CLinerBuilder = new CLinerBuilder();
        private static readonly GKeyValueBuilder GKeyValueBuilder = new GKeyValueBuilder();
        private static readonly FileUploadableBuilder FileUploadableBuilder = new FileUploadableBuilder();

        public FileScheduler GetNextFileForSchedule()
        {
            var obj = FileSchedulerBuilder.NextFileForSchedule();

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

                var schedulerDate = fileScheduler.FileDate.Date;
                var dependFileScheduler = FileSchedulerBuilder.DependOnAliasAndSheduleDate(schedulerDate,
                                                                                           dependAlias).ToList();

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
                    FileSchedulerBuilder.Save(fileScheduler);
                }
            }
            return file2Upload;
        }

        public int GetDoneFile(ColloSysEnums.FileAliasName aliasName, DateTime date)
        {
            return FileSchedulerBuilder.DoneFileCount(date, aliasName);
        }

        public void CommitData<TEntity>(IList<TEntity> data, FileScheduler file, IRowCounter counter)
                where TEntity : Entity, new()
        {

            if (data.Count == 0)
            {
                return;
            }

            _logger.Info(string.Format("ReadFile: updating filestatus table. " +
                           "rows uploaded {0}. valid {1}",
                           counter.GetTotalRecordCount(), counter.GetValidRecordCount()));

            var session = SessionManager.GetCurrentSession();
            foreach (var entity in data)
            {
                session.Save(entity);
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

            session.Save(status);

        }

        public IEnumerable<FileValueMapping> GetFieldValueMappings(Guid id)
        {
            return FileValueMappingBuilder.OnFileMappingId(id);
        }

        public void SetDoneStatus(FileScheduler fileScheduler, IRowCounter counter)
        {
            //var fx = session.Get<FileScheduler>(fileScheduler.Id);
            FileSchedulerBuilder.Refresh(fileScheduler);

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

            FileSchedulerBuilder.Save(fileScheduler);
        }

        public void ChangeStatus(FileScheduler fileScheduler)
        {

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

            FileSchedulerBuilder.Save(fileScheduler);
        }

        public List<TEntity> GetDataForDate<TEntity>(DateTime dateTime)
            where TEntity : Entity, IFileUploadable
        {
            return FileUploadableBuilder.GetDataForDate<TEntity>(dateTime).ToList();
        }

        public List<string> GetAccountNosForDate<TEntity>(DateTime dateTime)
           where TEntity : Entity, IUniqueKey
        {
            return FileUploadableBuilder.GetAccountNosForDate<TEntity>(dateTime);
        }

        public IList<TEntity> GetTableData<TEntity>()
             where TEntity : Entity
        {
            return FileUploadableBuilder.GetTableData<TEntity>();
        }

        public IList<TEntity> GetDataForPreviousDay<TEntity>(ColloSysEnums.FileAliasName aliasName,
        DateTime date, uint filecount)
            where TEntity : Entity, IFileUploadable
        {
            var lastuploadedfile = FileSchedulerBuilder.LastUploadedFiles(aliasName, date, filecount).ToList();

            if (lastuploadedfile.Count <= 0)
            {
                return new List<TEntity>();
            }
            var maxdate = lastuploadedfile.Max(x => x.FileDate);
            var lastuploadedfileid = lastuploadedfile.Where(x => x.FileDate == maxdate).Select(x => x.Id);

            var data = FileUploadableBuilder.GetDataForPreviousDay<TEntity>(lastuploadedfileid);

            return data;
        }

        public IList<string> GetValuesFromKey(ColloSysEnums.Activities area, string key)
        {
            return GKeyValueBuilder.ValueListOnAreaKey(area, key).ToList();
        }

        public bool SaveOrUpdateData<TEntity>(IEnumerable<TEntity> data)
            where TEntity : Entity
        {
            var dataList = data as IList<TEntity> ?? data.ToList();
            var session = SessionManager.GetCurrentSession();
            for (var i = 0; i < dataList.Count(); i += 1000)
            {
                var batchToSave = dataList.Skip(i).Take(1000);
                foreach (var entity in batchToSave)
                {
                    session.Save(entity);
                }
            }
            return true;
        }

        public bool MergeData<TEntity>(IEnumerable<TEntity> data)
           where TEntity : Entity, IFileUploadable, new()
        {
            var session = SessionManager.GetCurrentSession();

            foreach (var entity in data)
            {
                session.Merge(entity);
            }
            return true;
        }

        public void ResetFileStatus()
        {
            try
            {
                var fileSchedulerList = FileSchedulerBuilder.ResetFileStatus().ToList();

                foreach (var fileScheduler in fileSchedulerList)
                {
                    fileScheduler.UploadStatus = ColloSysEnums.UploadStatus.UploadRequest;
                    FileSchedulerBuilder.Save(fileScheduler);
                }

                _logger.Fatal(string.Format("DB Layer - ResetStatus: Resetting status of {0} files.",
                                          fileSchedulerList.Count));
            }
            catch (HibernateException e)
            {
                _logger.Error("ChangeStatus Exception : " + e);
            }
        }

        public Dictionary<string, Decimal> GetUnbilledAmounts(DateTime date)
        {
            var unbilledAmounts = new Dictionary<string, decimal>();
            CUnbilledBuilder.OnFileDate(date)
                   .ForEach(props => unbilledAmounts.Add((string)props[0], (decimal)props[1]));
            return unbilledAmounts;
        }

        public Dictionary<string, Decimal> GetCustTotalDue(DateTime date)
        {
            var dueAmount = new Dictionary<string, decimal>();
            CLinerBuilder.OnFileDate(date)
                         .ForEach(props => dueAmount.Add((string)props[0], (decimal)props[1]));
            return dueAmount;
        }

        public ISet<string> GetZGlobalCustId(DateTime date)
        {
            var result = CLinerBuilder.GlobelCustIdList(date);
            return new HashSet<string>(result);
        }
    }
}
