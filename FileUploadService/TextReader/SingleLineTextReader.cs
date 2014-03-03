#region references

using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using System;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.FileUploadService.UtilityClasses;
using NLog;

#endregion


namespace ColloSys.FileUploadService.TextReader
{
    internal abstract class SingleLineTextReader<TSingle> : TextReader<string>
        where TSingle : Entity, IFileUploadable, IUniqueKey, new()
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected readonly Dictionary<string, TSingle> TodayRecordList = new Dictionary<string, TSingle>();

        #region constructor

        private static FileReaderProperties GetFileReaderProperties(FileReaderProperties props)
        {
            props.HasMultiLineRecords = false;
            return props;
        }

        protected SingleLineTextReader(FileScheduler file, FileReaderProperties properties)
            : base(file, GetFileReaderProperties(properties))
        {
        }

        #endregion

        #region abstract member

        protected abstract TSingle GetRecord(string row);

        protected abstract bool IsRecordValid(TSingle record);

        protected abstract void PopulateDefault(TSingle record);

        protected virtual TSingle GetByUniqueKey(TSingle record)
        {
            return null;
        }

        protected abstract bool PerformUpdates(TSingle record);

        #endregion

        #region Save
        private TSingle GetValidRecord(string row)
        {
            try
            {
                var record = GetRecord(row);

                if (!IsRecordValid(record))
                    return null;

                PopulateDefault(record);

                return record;
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }

            return null;
        }

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

                IList<TSingle> savedRows = new List<TSingle>();

                foreach (var row in rowList)
                {
                    var newVersion = GetValidRecord(row.RowValue);
                    var rowNo = row.LineNo;

                    if (newVersion == null)
                    {
                        Reader.Counter.AddIgnoredRecord(rowNo);
                        continue;
                    }

                    Reader.Counter.AddValidRecord(rowNo);

                    var oldVersion = GetByUniqueKey(newVersion);

                    if (oldVersion != null)
                    {
                        Reader.Counter.AddDuplicateRecord(rowNo);
                        newVersion.CloneUniqueProperties(oldVersion);
                        continue;
                    }

                    //var updateHelper = ((oldVersion == null) || PerformUpdates(oldVersion));

                    //if (!updateHelper)
                    //{
                    //    validData++;
                    //    continue;
                    //}
                    newVersion.FileRowNo = rowNo;
                    ((IFileUploadable)newVersion).FileDate = Reader.UploadedFile.FileDate.Date;
                    newVersion.FileScheduler = Reader.UploadedFile;
                    savedRows.Add(newVersion);
                    TodayRecordList[newVersion.AccountNo] = newVersion;
                    Reader.Counter.AddUploadRecord(rowNo);
                }

                Reader.GetDataLayer.CommitData(savedRows, Reader.UploadedFile, Reader.Counter);

                _log.Debug("ActualUpload: saved the batch.");
                rowList.Clear();
                errorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _log.Warn(string.Format("SaveRowList Gives Error : {0}", ex));
            }
        }

        #endregion
    }
}
