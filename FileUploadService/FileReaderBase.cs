#region region

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
using ColloSys.DataLayer.Services.FileUpload;
using ColloSys.DataLayer.Services.Shared;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploadService.Implementers;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.Types4Product;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;

#endregion

namespace ColloSys.FileUploadService
{
    public class FileReaderBase : IFileReader
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region interface - properties

        private IDBLayer _dataLayer = new DbLayer();
        public IDBLayer GetDataLayer
        {
            get { return _dataLayer ?? (_dataLayer = new DbLayer()); }
        }

        private const int BatchSize = 1000;
        public int GetBatchSize
        {
            get { return BatchSize; }
        }

        private FileInfo _inputFile;
        public FileInfo GetInputFile
        {
            get
            {
                return _inputFile ??
                    (_inputFile = new FileInfo(UploadedFile.FileDirectory + "\\" + UploadedFile.FileName));
            }
        }

        private FileScheduler _currentFile;
        public FileScheduler UploadedFile
        {
            get { return _currentFile; }
            private set
            {
                if ((value == null) || (value.Id == Guid.Empty))
                {
                    throw new InvalidDataException("file scheduler must be non-null. and must have non-empty id.");
                }

                _currentFile = value;
            }
        }

        #endregion

        #region ctor/initializer

        public FileReaderProperties Properties { get; private set; }

        private IFileReaderNeeds ReaderNeeds { get; set; }
        public IRowCounter Counter { get; private set; }

        //public IList<DateTime> FileDateList { get; private set; }

        private bool _isInitialized;
        public void InitFileReader(FileScheduler file, FileReaderProperties properties, IFileReaderNeeds reader)
        {
            if (_isInitialized)
            {
                throw new InvalidProgramException("Cannot initialize same file twice.");
            }

            UploadedFile = file;
            Counter = new RowCounter(!properties.HasMultiLineRecords);
            //FileDateList = new List<DateTime>();
            //if (!properties.HasFileDateInsideFile)
            //{
            //    FileDateList.Add(UploadedFile.FileDate.Date);
            //}
            ReaderNeeds = reader;

            Properties = properties;


            _isInitialized = true;
        }

        #endregion

        #region interface - methods

        public void UploadFile()
        {
            if (!_isInitialized)
            {
                throw new InvalidProgramException("Please call Initialize method before uploading the file.");
            }

            _logger.Info("FileReaderBase: validating file :" + GetInputFile.Name);
            string errorMessage;
            if (!IsFileValid(out errorMessage) || !ReaderNeeds.ReadFile(out errorMessage))
            {
                UploadedFile.StatusDescription = errorMessage;
                UploadedFile.UploadStatus = ColloSysEnums.UploadStatus.Error;
                GetDataLayer.ChangeStatus(UploadedFile);
                _logger.Error("FileReaderBase: file is not valid. reason : " + errorMessage);
                return;
            }

            _logger.Info("FileReaderBase: uploading file : " + UploadedFile.FileName);

            while (!ReaderNeeds.HasEofReached())
            {
                _logger.Info(string.Format("BatchProcessing : Starting with next batch."));

                ReaderNeeds.EnqueueRowList();
                _logger.Info(string.Format("BatchProcessing : EnqueueRowList() Done."));
                ReaderNeeds.SaveRowList(out errorMessage);
                _logger.Info(string.Format("BatchProcessing : SaveRowList() Done."));
                ReaderNeeds.SaveErrorTable();
                _logger.Info(string.Format("BatchProcessing : SaveErrorTable() Done."));

                _logger.Debug(string.Format("BatchProcessing : Record Status : {0}", Counter.GetRecordStatusAsString()));
            }

            _logger.Info(string.Format("BatchProcessing : PostProcessing Start"));
            UploadedFile.UploadStatus = ColloSysEnums.UploadStatus.PostProcessing;
            GetDataLayer.ChangeStatus(UploadedFile);
            ReaderNeeds.PostProcesing();
            _logger.Info(string.Format("BatchProcessing : PostProcessing() Done"));

            _logger.Info("ReadFile: Retry error record.");
            ReaderNeeds.RetryErrorRows();

            _logger.Info("ReadFile: saving the error table.");
            SaveDoneStatus();

            _logger.Info("ReadFile: send status mail");
            //SendStatusMail(UploadedFile);
            
        }

        private bool IsFileValid(out string errorMessage)
        {
            var expectedExt = "." + Enum.GetName(typeof(ColloSysEnums.FileType), UploadedFile.FileDetail.FileType);
            IFileValidator reader = new FileValidator(GetInputFile, (long)UploadedFile.FileSize, expectedExt);

            return reader.IsFileValid(out errorMessage);
        }

        private static void SendStatusMail(FileScheduler scheduler)
        {
            try
            {
                // get file scheduler from db
                var session = SessionManager.GetCurrentSession();
                var fileScheduler = session.QueryOver<FileScheduler>()
                                           .Fetch(x => x.FileDetail).Eager
                                           .Where(x => x.Id == scheduler.Id)
                                           .SingleOrDefault();

                if (fileScheduler == null)
                {
                    _logger.Fatal("FileStatus: DownloadFile: no file scheduler entry found.");
                    throw new InvalidDataException("No such file found.");
                }

                var fileInfo = DownloadFile(fileScheduler);
                var subject = string.Format("File : {0}, of Date : {1} is uploaded with status : {2}", fileScheduler.FileName,
                                            fileScheduler.FileDate, fileScheduler.UploadStatus);
                EmailService.EmailReport(fileInfo, fileScheduler.FileDetail.EmailId, subject);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("SendStatusMail :", exception);
            }
        }

        private void SaveDoneStatus()
        {
            _logger.Info("ReadFile: updating the file status to Done.");
            GetDataLayer.SetDoneStatus(UploadedFile, Counter);
        }

        public static FileInfo DownloadFile(FileScheduler fileScheduler)
        {
            // get data for that filescheduler from db 
            var uploadableentity = ClassType.GetClientDataClassObjectByTableName(fileScheduler.FileDetail.ActualTable);
            var entityName = uploadableentity.GetType().Name;
            var fileschdulerName = uploadableentity.GetType().GetProperties()
                                                   .Single(x => x.PropertyType == typeof(FileScheduler)).Name;
            IList result;
            try
            {
                _logger.Info(string.Format("FileStatus: DownloadFile: download data for {0}."
                                           , fileScheduler.FileDetail.ActualTable));
                var session = SessionManager.GetCurrentSession();
                var criteria = session.CreateCriteria(uploadableentity.GetType(), entityName);
                criteria.CreateCriteria(fileschdulerName, fileschdulerName, JoinType.InnerJoin);
                criteria.Add(Restrictions.Eq(string.Format("{0}.{1}.Id", entityName, fileschdulerName), fileScheduler.Id));
                _logger.Info("FileStatus: DownloadFile: criteria =>" + criteria);
                result = criteria.List();
                _logger.Fatal("FileStatus: DownloadFile: total rows to write in excel : " + result.Count);
            }
            catch (HibernateException exception)
            {
                _logger.ErrorException("Error occured while executing command : " + exception.Data, exception);
                throw new Exception("NHibernate Error : " + (exception.InnerException != null
                                                                 ? exception.InnerException.Message
                                                                 : exception.Message));
            }

            // create excel from data
            var filename = Regex.Replace(fileScheduler.FileName.Substring(16), @"[^\w]", "_");
            var outputfilename = string.Format("output_{0}_{1}.xlsx", filename, DateTime.Now.ToString("HHmmssfff"));
            var file = new FileInfo(Path.GetTempPath() + outputfilename);
            _logger.Info(string.Format("FileStatus: DownloadFile: generating file from {0} for {1}, date {2}"
                    , entityName, fileScheduler.Id, fileScheduler.FileDate.ToShortDateString()));
            try
            {
                var includeList = FileColumnService.GetColumnDetails(fileScheduler.FileDetail.AliasName);
                if (includeList.Count == 0)
                    includeList = GetColumnsToWrite(uploadableentity, fileScheduler.FileDetail.AliasName);

                ClientDataWriter.ListToExcel(result, file, includeList);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("FileStatus : could not generate excel. ", exception);
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
            foreach (var info in props.Where(x => !exludeList.Contains(x.Name)))
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


        #endregion
    }
}