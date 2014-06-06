#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.FileUploadService.UtilityClasses;
using ColloSys.Shared.ErrorTables;
using NLog;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    public abstract class SingleTableExcelReader<TEntity> : ExcelReader<TEntity>
            where TEntity : Entity, IFileUploadable, IUniqueKey, new()
    {
        #region ctor

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly FileStatus _lastFileStatus;
        protected readonly MultiKeyEntityList<TEntity> TodayRecordList;

        private static FileReaderProperties GetFileReaderProperties(FileReaderProperties props)
        {
            props.HasMultiLineRecords = false;
            return props;
        }

        protected SingleTableExcelReader(FileScheduler file, FileReaderProperties properties)
            : base(file, GetFileReaderProperties(properties))
        {
            TodayRecordList = new MultiKeyEntityList<TEntity>();

            _lastFileStatus = Reader.UploadedFile.FileStatuss.OrderBy(x => x.TotalRows).LastOrDefault();
            Reader.Counter.InitializeCounter(_lastFileStatus);
        }

        #endregion

        #region abstract methods

        public abstract bool PopulateComputedValue(TEntity record, out string errorDescription);

        public virtual bool CheckBasicField(DataRow dr)
        {
            return true;
        }

        public virtual bool IsRecordValid(TEntity record, out string errorDescription)
        {
            errorDescription = string.Empty;
            return true;
        }

        public virtual TEntity GetByUniqueKey(TEntity record)
        {
            return null;
        }

        protected virtual void AddRecordInListByUniqueKey(TEntity record, List<TEntity> recordList)
        {
            recordList.Add(record);
        }

        public virtual bool PerformUpdates(TEntity record)
        {
            return true;
        }

        #endregion

        #region Get Next Batch

        protected override List<TEntity> GetNextBatch()
        {
            var recordList = new List<TEntity>();
            var rows = GetNextRows().ToArray();
            _log.Info(string.Format("Rows return by GetNextRows() : {0}", rows.Count()));

            foreach (var dr in rows)
            {
                RowNo++;

                if (_lastFileStatus != null && _lastFileStatus.TotalRows >= RowNo)
                    continue;

                dr[UploaderConstants.ErrorFileRowNo] = RowNo;

                string errorDescription;
                var record = GetTRecord(dr, out errorDescription);

                if (record == null)
                {
                    if (!string.IsNullOrWhiteSpace(errorDescription))
                    {
                        //Reader.Counter.AddErrorRecord(0);
                        AddInErrorTable(dr, errorDescription);
                        Reader.Counter.AddErrorRecord(RowNo);
                    }

                    continue;
                }

                //record.FileRowNo = RowNo;
                ValidRecordBatchTable.Rows.Add(ValidRecordBatchTable.NewRow().ItemArray = dr.ItemArray);
                Reader.Counter.AddValidRecord(RowNo);
                AddRecordInListByUniqueKey(record, recordList);
            }

            _log.Info(string.Format("Total Valid Record Created : {0}, by Total Rows : {1}", recordList.Count, rows.Count()));

            return recordList;
        }

        public override TEntity GetTRecord(DataRow dr, out string errorDescription)
        {
            var record = new TEntity();

            if (!CheckBasicField(dr))
            {
                Reader.Counter.AddIgnoredRecord(RowNo);
                errorDescription = string.Empty;
                return null;
            }

            if (!GetRecord(dr, record, out errorDescription))
                return null;

            if (!PopulateComputedValue(record, out errorDescription))
                return null;

            if (!IsRecordValid(record, out errorDescription))
            {
                Reader.Counter.AddIgnoredRecord(RowNo);
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dr[UploaderConstants.ErrorFileRowNo].ToString()))
            {
                record.FileRowNo = Convert.ToUInt64(dr[UploaderConstants.ErrorFileRowNo]);
            }

            return record;
        }

        #endregion

        #region Save
        public override void SaveRowList(out string errorMessage)
        {
            try
            {
                var rowList = DequeueRowList();
                if (rowList.Count == 0)
                {
                    errorMessage = string.Empty;
                    return;
                }

                IList<TEntity> savedRows = new List<TEntity>();

                foreach (var newVersion in rowList)
                {
                    var oldVersion = GetByUniqueKey(newVersion);

                    if (oldVersion != null)
                    {
                        Reader.Counter.AddDuplicateRecord(RowNo);
                        newVersion.CloneUniqueProperties(oldVersion);
                    }

                    //var updateHelper = ((oldVersion == null) || PerformUpdates(oldVersion));

                    //if (!updateHelper)
                    //{
                    //    validData++;
                    //    continue;
                    //}

                    // if file date is empty set date when file is uploaded
                    if (((IFileUploadable)newVersion).FileDate < DateTime.Today.AddYears(-100))
                    {
                        ((IFileUploadable)newVersion).FileDate = Reader.UploadedFile.FileDate;
                    }

                    newVersion.FileScheduler = Reader.UploadedFile;
                    savedRows.Add(newVersion);
                    //OldDbRecordList.Add(newVersion);
                    TodayRecordList.AddEntity(newVersion);
                    Reader.Counter.AddUploadRecord(RowNo);
                }

                Reader.GetDataLayer.CommitData(savedRows, Reader.UploadedFile, Reader.Counter);

                _log.Debug("ActualUpload: saved the batch.");
                rowList.Clear();
                ValidRecordBatchTable.Clear();
                errorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                AddValidBatchTableToErrorTable(ex.Message);
                _log.Error("SaveRowList : " + ex);
            }
        }

        #endregion

        #region Save Single Row

        public override bool SaveTRecord(TEntity newVersion, out string errorMessage)
        {
            try
            {
                using (var session = SessionManager.GetNewSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        var oldVersion = GetByUniqueKey(newVersion);

                        if (oldVersion != null)
                        {
                            newVersion.CloneUniqueProperties(oldVersion);
                            session.Evict(oldVersion);
                        }

                        var updateHelper = ((oldVersion == null) || PerformUpdates(oldVersion));

                        if (!updateHelper)
                        {
                            errorMessage = string.Empty;
                            return true;
                        }

                        // if file date is empty set date when file is uploaded
                        if (((IFileUploadable)newVersion).FileDate < DateTime.Today.AddYears(-10))
                        {
                            ((IFileUploadable)newVersion).FileDate = Reader.UploadedFile.FileDate;
                        }

                        var fileScheduler = session.Load<FileScheduler>(Reader.UploadedFile.Id);
                        newVersion.FileScheduler = fileScheduler;
                        session.SaveOrUpdate(newVersion);

                        tx.Commit();

                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("SaveTRecord : " + ex);
                errorMessage = ex.Message;
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        #endregion
    }
}