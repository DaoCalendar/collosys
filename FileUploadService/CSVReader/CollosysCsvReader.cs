#region references

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.DataAccess;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.IComponents;
using ColloSys.DataLayer.Services.FileUpload;
using ColloSys.DataLayer.Shared;
using ColloSys.FileUploadService.BaseReader;
using ColloSys.FileUploadService.Reader;
using Excel;
using FileHelpers.DataLink;
using FileHelpers.Dynamic;
using NLog;
using DataTable = System.Data.DataTable;
using CsvHelper;
#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    internal abstract class CollosysCsvReader<T> : FileReaderBase where T : Entity, IFileUploadable, new()
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region ctor

        protected IList<FileMapping> FileMappingList;
        protected Guid FileSchedulerId { get; private set; }
        private readonly Queue<List<T>> _rowQueue;

        protected CollosysCsvReader(FileScheduler file)
            : base(file)
        {
            _rowQueue = new Queue<List<T>>();
        }

        protected CollosysCsvReader(Guid fileSchdulerId)
            : base(fileSchdulerId)
        {
            FileMappingList = FileMappingServices.GetFileMappings(FileDetail.TempTable);

            _log.Info(string.Format("For File {0} Total File Mapping : {1}", FileDetail.AliasName, FileMappingList.Count));
        }

        #endregion

        #region Error Record

        private DataTable ErrorRecordTable { get; set; }

        protected string GetErrorDescription(FileMapping fileMapping, string errorMessage)
        {
            var field = (fileMapping == null) ? "Record" : fileMapping.TempColumn;
            return string.Format("{0} is not valid. Error : {1}", field, errorMessage);
        }

        protected void AddInErrorTable(DataRow dr, string errorDescription)
        {
            dr[UploaderConstants.ErrorDescription] = errorDescription;
            dr[UploaderConstants.ErrorStatus] = EnumHelper.ErrorStatus.DataError;
            dr[UploaderConstants.ErrorFileUploaderId] = FileScheduled.Id;
            dr[UploaderConstants.ErrorPrimaryKey] = Guid.NewGuid();

            if (string.IsNullOrEmpty(Convert.ToString(dr[UploaderConstants.ErrorFileDate])))
                dr[UploaderConstants.ErrorFileDate] = FileDate;

            lock (ErrorRecordTable)
            {
                ErrorRecordTable.Rows.Add(ErrorRecordTable.NewRow().ItemArray = dr.ItemArray);

                if (ErrorRecordTable.Rows.Count <= BatchSize)
                    return;

                SaveErrorTable();
                ErrorRecordTable.Clear();
                //ErrorRecordTable.Columns.Remove("ROWID");// Remove ROWID column
            }
        }

        protected override void SaveErrorTable()
        {
            try
            {
                var connectionString = FileUploaderService.ConnectionString;// SessionManager.GetCurrentSession().Connection.ConnectionString;
                _log.Info(string.Format("Connection String for Bulk Upload : {0}", connectionString));
                var bulkCopySql = new BulkCopySql(connectionString, ErrorRecordTable);
                bulkCopySql.CopyDataIntoDbTable(ErrorRecordTable);
            }
            catch (Exception ex)
            {
                _log.Error("SaveErrorTable : " + ex.ToString());
                //Log.Warn(string.Format("SaveErrorTable Gives Error : {0}", ex.Message));
            }
        }


        #endregion

        #region Read File Table
        private int _batchCount;
        private DataTable _dataTable;

        private IEnumerable<DataRow> InputFileDataRows { get; set; }

        protected IEnumerable<DataRow> GetNextRows()
        {
            lock (InputFileDataRows)
            {
                var rowList = InputFileDataRows.Skip(BatchSize * (_batchCount++))
                                                                .Take(BatchSize);

                return rowList;

                //string.Compare((field as string).Trim(), string.Empty) == 0)) 

                //return rowList.Where(row => row[0] != null && row[1] != null);
            }
        }

        protected override bool HasEofReached()
        {
            lock (InputFileDataRows)
            {
                return InputFileDataRows == null || (InputFileDataRows.Count() <= BatchSize * _batchCount);
                //!InputFileDataRows.GetEnumerator().MoveNext();
                // ;
            }
        }

        #endregion

        #region Read File
        protected StreamReader InputFileStream { get; private set; }

        protected override void OpenFile()
        {
            try
            {
                if (!IsFileValid())
                    return;

                var memoryMapped = MemoryMappedFile.CreateFromFile(InputFile.FullName, FileMode.Open, "TempTextFile", InputFile.Length);//65536, 64 mb
                InputFileStream = new StreamReader(memoryMapped.CreateViewStream());
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            // ReSharper restore EmptyGeneralCatchClause
        }


        private enum FileUploadBy
        {
            FileHelper,
            OleDbProvider,
            ExcelReader,
            CsvHelper
        }

        protected override void OpenFile()
        {
            try
            {
                if (IsFileValid())
                {
                    FileMappingList = FileMappingServices.GetFileMappings(FileDetail.TempTable);
                    _log.Info(string.Format("For File {0} Total File Mapping : {1}", FileDetail.AliasName, FileMappingList.Count));

                    //_dataTable = GetTempTable();
                    DataTable fileTable;
                    FileUploadBy userFileHelper;
                    if (!Enum.TryParse(ConfigurationManager.AppSettings["UseFileHelper"], out userFileHelper))
                    {
                        return;
                    }

                    switch (userFileHelper)
                    {
                        case FileUploadBy.FileHelper:
                            _log.Info("Read excel file by file helper");
                            fileTable = GenerateDataTablebyFileHelper();
                            break;
                        case FileUploadBy.OleDbProvider:
                            _log.Info("Read excel file by OleDb");
                            fileTable = GenerateDataTableByOleDb();
                            break;
                        case FileUploadBy.ExcelReader:
                            _log.Info("Read excel file by OleDb");
                            fileTable = GenerateDataTableByExcelReader();
                            break;
                        case FileUploadBy.CsvHelper:
                            _log.Info("Read excel file by OleDb");
                            fileTable = GenerateDataTableByCsvHelper();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _dataTable = fileTable.AsEnumerable().Where(row =>
                                                !row.ItemArray.All(field => field is DBNull
                                                    || string.IsNullOrWhiteSpace(field as string)))
                                                .CopyToDataTable();

                    // reset column name in data table
                    foreach (var fileColumn in FileDetail.FileColumns)
                    {
                        if (fileColumn.Position != 0)
                        {
                            _dataTable.Columns[(int)fileColumn.Position - 1].ColumnName = fileColumn.TempColumnName;
                        }
                    }

                    // add expression column in data table
                    var expMapping = FileMappingList.Where(m => m.ValueType == EnumHelper.FileMappingValueType.ExpressionValue);
                    foreach (var mapping in expMapping)
                    {
                        var dc = new DataColumn(mapping.ActualColumn, typeof(string), mapping.DefaultValue);
                        _dataTable.Columns.Add(dc);
                    }

                    // add another columns 
                    if (!_dataTable.Columns.Contains(UploaderConstants.ErrorFileDate))
                        _dataTable.Columns.Add(UploaderConstants.ErrorFileDate);

                    _dataTable.Columns.Add(UploaderConstants.ErrorDescription);
                    _dataTable.Columns.Add(UploaderConstants.ErrorStatus);
                    _dataTable.Columns.Add(UploaderConstants.ErrorFileUploaderId, typeof(Guid));
                    _dataTable.Columns.Add(UploaderConstants.ErrorPrimaryKey, typeof(Guid));
                    _dataTable.Columns[UploaderConstants.ErrorPrimaryKey].SetOrdinal(0);

                    TotalRows = (ulong)_dataTable.Rows.Count;
                    InputFileDataRows = _dataTable.AsEnumerable();
                    ErrorRecordTable = _dataTable.Clone();
                    ErrorRecordTable.TableName = string.Format(FileDetail.ErrorTable);
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception ex)
            {
                _log.Error("OpenFile : " + ex.ToString());
                ErrorMessage = ex.Message;
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        private DataTable GenerateDataTableByCsvHelper()
        {
            DataTable dt = new DataTable();
            foreach (var fileColumn in FileDetail.FileColumns.OrderBy(c => c.Position))
            {
                if (fileColumn.Position != 0)
                {
                    dt.Columns.Add(fileColumn.TempColumnName);
                }
            }

            System.IO.TextReader readFile = new StreamReader(InputFile.FullName);
            var csv = new CsvReader(readFile);
            csv.Configuration.HasHeaderRecord = true;

            while (csv.Read())
            {
                var dataRow = dt.NewRow();
                var length = (dt.Columns.Count < csv.FieldHeaders.Count()) ? dt.Columns.Count : csv.FieldHeaders.Count();
                for (int i = 0; i < length; i++)
                {
                    dataRow[i] = csv.GetField(i);
                }
                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        private DataTable GenerateDataTableByOleDb()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + InputFile.FullName
                + ";Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1;\"";//TypeGuessRows=0;


            //var testingData = ReadExcel(InputFile.FullName);

            var sheetData = new DataTable();
            using (var conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                var cmd = new OleDbCommand(string.Format("select * from [{0}]", FileDetail.SheetName), conn);

                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr != null) sheetData.Load(dr);
                conn.Close();
            }

            sheetData.Rows[0].Delete();
            sheetData.AcceptChanges();

            return sheetData;
        }

        private DataTable GenerateDataTableByExcelReader()
        {
            FileStream stream = File.Open(InputFile.FullName, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;
            if (InputFile.Extension == ".xls")
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (InputFile.Extension == ".xlsx")
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                return null;
            }

            ////1. Reading from a binary Excel file ('97-2003 format; *.xls)
            //var excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            ////...
            ////2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            //var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //...
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            //var result = excelReader.AsDataSet();

            //4. DataSet - Create column names from first row
            excelReader.IsFirstRowAsColumnNames = true;
            return excelReader.AsDataSet().Tables[0];
        }

        private DataTable GenerateDataTablebyFileHelper()
        {
            var cb = new DelimitedClassBuilder(string.Format("{0}_VerticalBar", FileDetail.TempTable), "|")
            {
                IgnoreFirstLines = 1,
                IgnoreEmptyLines = true
            };

            var fileColumns =
                FileColumnServices.GetTempColumns(FileDetail.AliasName).OrderBy(c => c.Position);

            foreach (var fileColumn in fileColumns)
            {
                //if (fileColumn.ColumnDataType == EnumHelper.FileDataType.Date)
                //{
                //    cb.AddField(fileColumn.TempColumnName, typeof(DateTime?));
                //    cb.LastField.FieldNullValue = null;
                //    //cb.LastField.Converter.Kind = ConverterKind.Date;
                //    //cb.LastField.Converter.Arg1 = fileColumn.DateFormat;
                //    continue;
                //}

                cb.AddField(fileColumn.TempColumnName, typeof(string));
            }

            //cb.AddField("First", typeof(string));
            //cb.AddField("Second", typeof(string));
            //cb.AddField("Age", typeof(int));


            cb.LastField.FieldQuoted = true;
            cb.LastField.QuoteChar = '"';


            var provider = new ExcelStorage(cb.CreateRecordClass())
            {
                StartRow = Convert.ToInt32(FileDetail.SkipLine + 2),
                StartColumn = 1,
                FileName = InputFile.FullName
            };

            var dataTable = provider.ExtractRecordsAsDT();

            return dataTable;
        }

        #endregion

        #region abstract member

        protected abstract List<T> GetNextBatch();

        protected abstract T GetTRecord(DataRow dr, out string errorDescription);

        protected abstract bool SaveTRecord(T newVersion, out string errorMessage);

        #endregion

        #region Read Record
        protected bool GetRecord<TRecord>(DataRow dr, TRecord record, out string errorDescription) where TRecord : Entity, new()
        {
            var infoFileMapping = FileMappingList.Where(m => m.ActualTable == record.GetType().Name
                                                        && m.ValueType != EnumHelper.FileMappingValueType.ComputedValue);

            foreach (var fileMapping in infoFileMapping)
            {
                try
                {
                    var prop = record.GetType().GetProperty(fileMapping.ActualColumn);

                    if (fileMapping.ValueType == EnumHelper.FileMappingValueType.DefaultValue)
                    {
                        prop.SetValueWithType(record, fileMapping.DefaultValue);
                        continue;
                    }

                    if (fileMapping.ValueType == EnumHelper.FileMappingValueType.ExpressionValue)
                    {
                        prop.SetValueWithType(record, dr[fileMapping.ActualColumn].ToString());
                        continue;
                    }

                    var dateFormat = FileDetail.FileColumns.Single(c => c.Position == fileMapping.Position).DateFormat;

                    prop.SetValueWithType(record, dr[fileMapping.TempColumn].ToString(), dateFormat);
                }
                catch (Exception ex)
                {
                    errorDescription = GetErrorDescription(fileMapping, ex.Message);
                    //errorOnMapping = fileMapping;

                    //AddInErrorTable(dr, fileMapping);

                    return false;
                }
            }

            //errorOnMapping = null;
            errorDescription = string.Empty;
            return true;
        }
        #endregion

        #region locking/unlocking
        protected override void EnqueueRowList()
        {
            var ci = new CultureInfo("en-IN");
            CultureInfo.DefaultThreadCurrentCulture = ci;
            CultureInfo.DefaultThreadCurrentUICulture = ci;

            var rowList = GetNextBatch();

            lock (_rowQueue)
            {
                _rowQueue.Enqueue(rowList);
            }
        }

        protected List<T> DequeueRowList()
        {
            // get string list from queue
            List<T> rowList;
            while (true)
            {
                if (_rowQueue.Count < 1)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                lock (_rowQueue)
                {
                    rowList = _rowQueue.Dequeue();
                }
                break;
            }

            return rowList;
        }
        #endregion

        #region For Save Error Currection

        protected bool SaveEditedErrorRow(DataRow dr, out string errorMessage)
        {
            string errorDescription;
            var record = GetTRecord(dr, out errorDescription);

            if (record == null)
            {
                errorMessage = errorDescription;
                return false;
            }

            return SaveTRecord(record, out errorMessage);
        }

        #endregion
    }
}

//int NextRowTake = BatchSize;
//int RemainingRows = (InputFileTable.Rows.Count - BatchSize * _batchCount);

//if (RemainingRows < NextRowTake)
//    NextRowTake = RemainingRows; 

//private DataTable GetTempTable()
//{
//    var dataSet = ReadExcel(InputFile.FullName);

//    var dataTable = dataSet.Tables[FileDetail.SheetName];

//    for (int i = 0; i < FileDetail.SkipLine; i++)
//        dataTable.Rows[i].Delete();

//    // reset column name in data table
//    foreach (FileColumn fileColumn in FileDetail.FileColumns)
//    {
//        if (fileColumn.Position != 0)
//        {
//            dataTable.Columns[(int)fileColumn.Position - 1].ColumnName = fileColumn.TempColumnName;
//        }
//    }

//    // add expression column in data table
//    var expMapping = FileMappingList.Where(m => m.ValueType == EnumHelper.FileMappingValueType.ExpressionValue);
//    foreach (var mapping in expMapping)
//    {
//        var dc = new DataColumn(mapping.ActualColumn, typeof(string), mapping.DefaultValue);
//        dataTable.Columns.Add(dc);
//    }

//    dataTable.AcceptChanges();

//    return dataTable;
//}

//public static DataSet ReadExcel(string fileName)
//{
//    FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);

//    //var sr = new StreamReader(stream);
//    //uint count = 1, line = 0;
//    //while (!sr.EndOfStream && line < count)
//    //{
//    //    sr.ReadLine();
//    //    line++;
//    //}

//    using (var excelReader = ExcelReaderFactory.CreateBinaryReader(stream, true))
//    {
//        excelReader.IsFirstRowAsColumnNames = false;
//        var test = excelReader.AsDataSet();
//        return test;
//    }
//}
