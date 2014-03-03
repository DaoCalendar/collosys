#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.FileUploadService.BaseReader;
using ColloSys.FileUploadService.Excel2DT;
using ColloSys.FileUploadService.Interfaces;
using ColloSys.Shared.SharedUtils;
using NLog;
using NPOI.SS.Formula.Functions;
using DataTable = System.Data.DataTable;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    public abstract class ExcelReader<T> : IFileReaderNeeds where T : Entity, IFileUploadable, new()
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region ctor

        //protected IList<FileMapping> FileMappingList;
        private IList<T> _rowQueue;
        protected readonly IFileReader Reader;
        protected ulong RowNo;

        protected ExcelReader(FileScheduler file, FileReaderProperties props)
        {
            Reader = new FileReaderBase();
            Reader.InitFileReader(file, props, this);
            _rowQueue = new List<T>();
            RowNo = 0;
        }

        #endregion

        #region Error Record

        public void UploadFile()
        {
            Reader.UploadFile();
        }


        private DataTable ErrorRecordTable { get; set; }

        protected DataTable ValidRecordBatchTable { get; private set; }

        public virtual bool PostProcesing()
        {
            return true;
        }

        protected string GetErrorDescription(FileMapping fileMapping, string errorMessage)
        {
            var field = (fileMapping == null) ? "Record" : fileMapping.TempColumn;
            return string.Format("{0} is not valid. Error : {1}", field, errorMessage);
        }

        protected void AddInErrorTable(DataRow dr, string errorDescription)
        {
            dr[UploaderConstants.ErrorDescription] = errorDescription;
            dr[UploaderConstants.ErrorStatus] = ColloSysEnums.ErrorStatus.DataError;
            dr[UploaderConstants.ErrorFileRowNo] = RowNo;
            dr[UploaderConstants.ErrorFileUploaderId] = Reader.UploadedFile.Id;
            dr[UploaderConstants.ErrorPrimaryKey] = Guid.NewGuid();

            if (string.IsNullOrEmpty(Convert.ToString(dr[UploaderConstants.ErrorFileDate])))
            {
                dr[UploaderConstants.ErrorFileDate] = Reader.UploadedFile.FileDate;
            }

            ErrorRecordTable.Rows.Add(ErrorRecordTable.NewRow().ItemArray = dr.ItemArray);
        }

        protected void AddValidBatchTableToErrorTable(string errorMessage)
        {
            foreach (DataRow dr in ValidRecordBatchTable.Rows)
            {
                Reader.Counter.AddErrorRecord(RowNo);
                AddInErrorTable(dr, errorMessage);
            }

            ValidRecordBatchTable.Clear();
        }

        public abstract void SaveRowList(out string errorMessage);

        public void SaveErrorTable()
        {
            try
            {
                var connectionString = FileUploaderService.ConnString;
                _log.Info(string.Format("Connection String for Bulk Upload : {0}", connectionString.ConnectionString));
                var bulkCopySql = new BulkCopySql(connectionString.ConnectionString);
                bulkCopySql.CopyDataIntoDbTable(ErrorRecordTable);

                _log.Debug("RecordStatus: " + Reader.Counter.GetRecordStatusAsString());
                ErrorRecordTable.Clear();
                ValidRecordBatchTable.Clear();
            }
            catch (Exception ex)
            {
                _log.Error("SaveErrorTable : " + ex);
            }
        }

       

        #endregion

        #region Read File Table
        private int _batchCount;
        private DataTable _dataTable;

        private IEnumerable<DataRow> InputFileDataRows { get; set; }

        protected IEnumerable<DataRow> GetNextRows()
        {
            return InputFileDataRows
                .Skip(Reader.GetBatchSize * (_batchCount++))
                .Take(Reader.GetBatchSize);
        }

        public bool HasEofReached()
        {
            return InputFileDataRows == null || (InputFileDataRows.Count() <= Reader.GetBatchSize * _batchCount);
        }

        #endregion

        #region Read File

        public bool ReadFile(out string errorMessage)
        {
            try
            {
                //FileMappingList = Reader.UploadedFile.FileDetail.FileMappings.ToList();
                _log.Info(string.Format("For File {0} Total File Mapping : {1}",
                    Reader.UploadedFile.FileDetail.AliasName, Reader.UploadedFile.FileDetail.FileMappings.Count));

                _log.Info("Reading excel file by:" + Reader.UploadedFile.FileDetail.FileReaderType);
                _dataTable = GenerateDataTable(Reader.UploadedFile.FileDetail.FileReaderType);

                var fileColumns =
                    Reader.UploadedFile.FileDetail.FileColumns.Where(fileColumn => fileColumn.Position != 0).ToArray();
                // rather than checking max of columns we have, check max of columns we need
                // if all columns we need are there, then no problem
                // this might err, only on computed column being out of max-range
                // to solve that issue, we need to specify the position for computed columns as well.
                var fileColumnsCount = Reader.UploadedFile.FileDetail.FileMappings.Max(x => x.Position);

                if (fileColumnsCount > _dataTable.Columns.Count)
                {
                    _log.Error(string.Format("Expected Columns({0}) and File Columns({1}) does not match", fileColumns.Count(), _dataTable.Columns.Count));

                    IList<string> columnname = new List<string>();
                    for (var i = 0; i < _dataTable.Columns.Count; i++)
                    {
                        columnname.Add(fileColumns.First(x => x.Position == i + 1).FileColumnName + ":" + _dataTable.Columns[i].ColumnName);
                    }
                    var names = string.Join(", ", columnname);
                    _log.Error(string.Format("Column names are : {0}", names));

                    throw new Exception(string.Format("Expected Columns({0}) and File Columns({1}) does not match", fileColumns.Count(), _dataTable.Columns.Count));
                }

                // reset column name in data table
                var columnCount = 0;
                foreach (var fileColumn in fileColumns)
                {
                    _dataTable.Columns[(int)fileColumn.Position - 1].ColumnName = fileColumn.TempColumnName;
                    columnCount++;
                }

                // Remove extra column
                var removeColumnCount = _dataTable.Columns.Count - columnCount;
                for (var i = 0; i < removeColumnCount; i++)
                {
                    _dataTable.Columns.RemoveAt(columnCount);
                    _dataTable.AcceptChanges();
                }

                // add expression column in data table
                var expMapping = Reader.UploadedFile.FileDetail.FileMappings
                                        .Where(m => m.ValueType == ColloSysEnums.FileMappingValueType.ExpressionValue);
                foreach (var mapping in expMapping)
                {
                    var dc = new DataColumn("Exp_" + mapping.ActualColumn, typeof(string), mapping.DefaultValue);
                    _dataTable.Columns.Add(dc);
                }

                // add error columns 
                if (!_dataTable.Columns.Contains(UploaderConstants.ErrorFileDate))
                    _dataTable.Columns.Add(UploaderConstants.ErrorFileDate);

                _dataTable.Columns.Add(UploaderConstants.ErrorDescription);
                _dataTable.Columns.Add(UploaderConstants.ErrorStatus);
                _dataTable.Columns.Add(UploaderConstants.ErrorFileRowNo, typeof(ulong));
                _dataTable.Columns.Add(UploaderConstants.ErrorFileUploaderId, typeof(Guid));
                _dataTable.Columns.Add(UploaderConstants.ErrorPrimaryKey, typeof(Guid));
                _dataTable.Columns[UploaderConstants.ErrorPrimaryKey].SetOrdinal(0);

                InputFileDataRows = _dataTable.AsEnumerable();
                ErrorRecordTable = _dataTable.Clone();
                ErrorRecordTable.TableName = string.Format(Reader.UploadedFile.FileDetail.ErrorTable);
                ValidRecordBatchTable = _dataTable.Clone();

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("OpenFile : " + ex);
                errorMessage = ex.Message;
                return false;
            }
        }

        private DataTable GenerateDataTable(ColloSysEnums.FileUploadBy uploadBy)
        {

            if (uploadBy == ColloSysEnums.FileUploadBy.NotSpecified)
            {
                switch (Reader.UploadedFile.FileDetail.FileType)
                {
                    case ColloSysEnums.FileType.csv:
                        uploadBy = ColloSysEnums.FileUploadBy.CsvHelper;
                        break;
                    case ColloSysEnums.FileType.txt:
                        break;
                    case ColloSysEnums.FileType.xls:
                        uploadBy = ColloSysEnums.FileUploadBy.NPOIXlsReader;
                        break;
                    case ColloSysEnums.FileType.xlsx:
                        uploadBy = ColloSysEnums.FileUploadBy.EpPlusXlsxReader;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("file extension : " + Reader.UploadedFile.FileDetail.FileType);
                }
            }

            switch (uploadBy)
            {
                case ColloSysEnums.FileUploadBy.FileHelper:
                    return FileHelperExcelReader.GenerateDataTable(Reader.UploadedFile,
                                                                   Reader.GetInputFile);
                case ColloSysEnums.FileUploadBy.OleDbProvider:
                    return OLEDBExcelReader.GenerateDataTable(Reader.UploadedFile,
                                                              Reader.GetInputFile);
                case ColloSysEnums.FileUploadBy.ExcelReader:
                    return ExcelNugetExcelReader.GenerateDataTable(Reader.GetInputFile);
                case ColloSysEnums.FileUploadBy.CsvHelper:
                    return CsvHelperExcelReader.GenerateDataTable(Reader.UploadedFile,
                                                                  Reader.GetInputFile,
                                                                  Reader.Properties.CsvDelimiter,
                                                                  Reader.Counter);
                case ColloSysEnums.FileUploadBy.NPOIXlsReader:
                    return NPOIExcelReader.GenerateDataTable(Reader.UploadedFile,
                                                        Reader.GetInputFile);
                case ColloSysEnums.FileUploadBy.EpPlusXlsxReader:
                    return EpPlusExcelsxReader.GenerateDataTable(Reader.UploadedFile,
                                                                     Reader.GetInputFile);
                default:
                    throw new ArgumentOutOfRangeException("uploadBy : " + uploadBy);
            }
        }

        #endregion

        #region abstract member

        protected abstract List<T> GetNextBatch();

        public abstract T GetTRecord(DataRow dr, out string errorDescription);

        public abstract bool SaveTRecord(T newVersion, out string errorMessage);

        #endregion

        #region Read Record
        protected bool GetRecord<TRecord>(DataRow dr, TRecord record, out string errorDescription)
                where TRecord : Entity, new()
        {
            var infoFileMapping = Reader.UploadedFile.FileDetail.FileMappings
                                        .Where(m => m.ActualTable == record.GetType().Name
                                                        && m.ValueType != ColloSysEnums.FileMappingValueType.ComputedValue);

            foreach (var fileMapping in infoFileMapping)
            {
                try
                {
                    var prop = record.GetType().GetProperty(fileMapping.ActualColumn);

                    switch (fileMapping.ValueType)
                    {
                        case ColloSysEnums.FileMappingValueType.DefaultValue:
                            if (fileMapping.DefaultValue != null)
                            {
                                prop.SetValueWithType(record, fileMapping.DefaultValue);
                            }
                            break;
                        case ColloSysEnums.FileMappingValueType.ExpressionValue:
                            prop.SetValueWithType(record, dr["Exp_" + fileMapping.ActualColumn].ToString());
                            break;
                        case ColloSysEnums.FileMappingValueType.ExcelValue:
                            var dateFormat = Reader.UploadedFile.FileDetail.FileColumns.Single(c => c.Position == fileMapping.Position).DateFormat;
                            prop.SetValueWithType(record, dr[(int)fileMapping.Position].ToString(), dateFormat);
                            break;
                        case ColloSysEnums.FileMappingValueType.MappedValue:
                            var mappings = Reader.GetDataLayer.GetFieldValueMappings(fileMapping.Id);
                            var inputValue = dr[(int)fileMapping.Position].ToString();
                            if (string.IsNullOrWhiteSpace(inputValue)) inputValue = "*";
                            var value = mappings.FirstOrDefault(x => x.SourceValue == inputValue);
                            var output = (value == null) ? null : value.DestinationValue;
                            prop.SetValueWithType(record, output);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    errorDescription = GetErrorDescription(fileMapping, ex.Message);
                    return false;
                }
            }

            errorDescription = string.Empty;
            return true;
        }
        #endregion


        #region Retry Error Rows
        public bool RetryErrorRows()
        {
            //fileScheduler = GetFileScheduler(fileScheduler.Id);

            string errorMessage;
            var dbLayer = new ErrorDbLayer(Reader.UploadedFile.FileDetail.ErrorTable);
            var dataTable = dbLayer.GetDataFromSchedulerId(Reader.UploadedFile.Id, out errorMessage);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                _log.Fatal(errorMessage);
                return false;
            }

            SessionManager.BindNewSession();

            foreach (DataRow errorRow in dataTable.Rows)
            {
                SaveEditedErrorRecord.SaveErrorRow(Reader.UploadedFile.FileDetail.AliasName, errorRow, out errorMessage);

                errorRow[UploaderConstants.ErrorDescription] = errorMessage;
                errorRow[UploaderConstants.ErrorStatus] = string.IsNullOrWhiteSpace(errorMessage) ?
                                                                ColloSysEnums.ErrorStatus.RetrySuccess.ToString() : ColloSysEnums.ErrorStatus.DataError.ToString();

                var errorDictionary = errorRow.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => errorRow.Field<object>(col.ColumnName));

                dbLayer.UpdateErrorData(errorDictionary, out errorMessage);
            }

            SessionManager.UnbindSession();
            return true;
        }

        #endregion


        #region locking/unlocking
        public void EnqueueRowList()
        {
            _log.Info("ActualUpload: get next batch.");
            _rowQueue = GetNextBatch();
        }

        protected IList<T> DequeueRowList()
        {
            _log.Info("ActualUpload: saving the batch. #record :" + _rowQueue.Count);
            return _rowQueue;
        }
        #endregion
    }
}