#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Services.FileUpload;
using ColloSys.QueryBuilder.FileUploadBuilder;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.Types4Product;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;

#endregion

namespace AngularUI.FileUpload.filestatus
{
    public static class FileStatusHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly FileSchedulerBuilder FileSchedulerBuilder=new FileSchedulerBuilder();

        public static IEnumerable<FileScheduler> GetOneWeekScheduledList()
        {
            var maxBackdate = DateTime.Today.AddMonths(-2);
            return FileSchedulerBuilder.OnMaxBackDate(maxBackdate);
        }

        public static IEnumerable<FileScheduler> GetScheduledList(DateTime fromDate,DateTime toDate)
        {
            return FileSchedulerBuilder.DateRange(fromDate, toDate);
        }

        public static FileScheduler ReScheduleFile(FileScheduler file)
        {
            if (file == null) return null;
            var entity =FileSchedulerBuilder.GetWithId(file.Id);
            entity.IsImmediate = true;
            entity.ImmediateReason = file.ImmediateReason;
            entity.StartDateTime = DateTime.Now;
            FileSchedulerBuilder.Save(entity);
            return entity;
        }

        public static FileScheduler RetryUpload(Guid id)
        {
            var scheduler = FileSchedulerBuilder.GetWithId(id);
            scheduler.UploadStatus = ColloSysEnums.UploadStatus.RetryUpload;
            scheduler.IsImmediate = true;
            scheduler.StartDateTime = DateTime.Now;
            scheduler.EndDateTime = null;
            scheduler.ImmediateReason = "retry upload";
            scheduler.StatusDescription = "retrying upload";
            FileSchedulerBuilder.Save(scheduler);
            return scheduler;
        }

        public static FileInfo DownloadFile(IEnumerable<FileScheduler> fileScheduler)
        {
            // get file scheduler from db
            var session = SessionManager.GetCurrentSession();
            var uploads = FileSchedulerBuilder.OnFileSchedulers(fileScheduler).ToList();

            if (uploads == null)
            {
                Logger.Fatal("FileStatus: DownloadFile: no file scheduler entry found.");
                throw new InvalidDataException("No such file found.");
            }

            // get data for that filescheduler from db 
            var uploadableentity = ClassType.GetClientDataClassObjectByTableName(uploads.First().FileDetail.ActualTable);
            var entityName = uploadableentity.GetType().Name;
            var fileschdulerName = uploadableentity.GetType().GetProperties()
                                                   .Single(x => x.PropertyType == typeof(FileScheduler)).Name;
            IList result;
            try
            {
                Logger.Info(string.Format("FileStatus: DownloadFile: download data for {0} files with id : {1}."
                                          , uploads.Count, string.Join(",", uploads.Select(x => x.Id).ToList())));
                var criteria = session.CreateCriteria(uploadableentity.GetType(), entityName);
                criteria.CreateCriteria(fileschdulerName, fileschdulerName, JoinType.InnerJoin);
                criteria.Add(Restrictions.In(string.Format("{0}.{1}.Id", entityName, fileschdulerName),
                                             uploads.Select(x => x.Id).Distinct().ToList()));
                Logger.Info("FileStatus: DownloadFile: criteria =>" + criteria);
                result = criteria.List();
                Logger.Fatal("FileStatus: DownloadFile: total rows to write in excel : " + result.Count);
            }
            catch (HibernateException exception)
            {
                Logger.ErrorException("Error occured while executing command : " + exception.Data, exception);
                throw new Exception("NHibernate Error : " + (exception.InnerException != null
                                                                 ? exception.InnerException.Message
                                                                 : exception.Message));
            }

            // create excel from data
            var filename = Regex.Replace(uploads.First().FileNameDisplay, @"[^\w]", "_");
            var outputfilename = string.Format("output_{0}_{1}.xlsx", filename, DateTime.Now.ToString("HHmmssfff"));
            var file = new FileInfo(Path.GetTempPath() + outputfilename);
            Logger.Info(string.Format("FileStatus: DownloadFile: generating file from {0} for {1}, date {2}"
                    , entityName, uploads.First().Id, uploads.First().FileDate.ToShortDateString()));
            try
            {
                var includeList = FileColumnService.GetColumnDetails(uploads.First().FileDetail.AliasName);
                if (includeList.Count == 0)
                    includeList = GetColumnsToWrite(uploadableentity, uploads.First().FileDetail.AliasName);

                ClientDataWriter.ListToExcel(result, file, includeList);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("FileStatus : could not generate excel. ", exception);
                throw new ExternalException("Could not generate excel. " + exception.Message);
            }

            return file;
        }

        private static IList<ColumnPositionInfo> GetColumnsToWrite(Entity uploadableentity, ColloSysEnums.FileAliasName aliasName)
        {
            IList<string> exludeList = new List<string>();
            if (uploadableentity.GetType().GetInterfaces().Contains(typeof(IFileUploadable)))
            {
                var entity = uploadableentity as IFileUploadable;
                if (entity != null) exludeList = entity.GetExcludeInExcelProperties();
            }

            var includeList = new List<ColumnPositionInfo>();
            var props = uploadableentity.GetType().GetProperties();
            uint position = 1;
            foreach (var info in props.Where( x=> !exludeList.Contains(x.Name)))
            {
                includeList.Add(new ColumnPositionInfo
                {
                    FieldName = info.Name,
                    DisplayName = info.Name,
                    Position = (++position),
                    WriteInExcel = true,
                    IsFreezed = false,
                    UseFieldNameForDisplay = true
                });
            }

            return includeList;
        }

    }

}

