#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.FileUploadService.BaseReader;
using NLog;
using Newtonsoft.Json.Linq;
using UserInterfaceAngular.NgGrid;

#endregion

namespace ColloSys.UserInterface.Areas.ErrorCorrection.ViewModels
{
    public static class ErrorViewModel
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        #region Get Ng Grid Options

        public static NgGridOptions GetErrorNgGrid(Guid id)
        {
            var fileScheduler = GetFileScheduler(id);

            var gridOptions = new NgGridOptions
                {
                    data = ErrorData(fileScheduler)
                };
            gridOptions.columnDefs.AddRange(ErrorNgGridColumns(fileScheduler.FileDetail));

            return gridOptions;
        }

        public static NgGridOptions GetApproverErrorNgGrid(Guid id)
        {
            var fileScheduler = GetFileScheduler(id);

            var gridOptions = new NgGridOptions
                {
                    data = ErrorDataForApprove(fileScheduler)
                };
            gridOptions.columnDefs.AddRange(ErrorNgGridColumns(fileScheduler.FileDetail));
            gridOptions.multiSelect = true;
            gridOptions.showSelectionCheckbox = true;

            return gridOptions;
        }

        private static IEnumerable<ColumnDef> ErrorNgGridColumns(FileDetail fileDetail)
        {
            var editableColumns = fileDetail.FileMappings.Where(x => x.Position != 0).Select(x => x.TempColumn);

            var columns = fileDetail.FileColumns.Select(fileColumn => new ColumnDef
                            {
                                field = fileColumn.TempColumnName,
                                displayName = fileColumn.FileColumnName,
                                readOnly = !editableColumns.Contains(fileColumn.TempColumnName)
                            }).ToList();

            columns.Add(new ColumnDef
                {
                    field = UploaderConstants.ErrorDescription,
                    displayName = UploaderConstants.ErrorDescription,
                    readOnly = true
                });

            columns.Add(new ColumnDef
            {
                field = UploaderConstants.ErrorFileRowNo,
                displayName = UploaderConstants.ErrorFileRowNo,
                readOnly = true
            });

            return columns;
        }

        #endregion

        #region Get Error Data

        private static IEnumerable<JObject> ErrorData(FileScheduler fileScheduler)
        {
            string errorMessage;
            var dbLayer = new ErrorDbLayer(fileScheduler.FileDetail.ErrorTable);
            var dataTable = dbLayer.GetDataFromSchedulerId(fileScheduler.Id, out errorMessage);


            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Log.Fatal(errorMessage);
                throw new Exception(errorMessage);
            }

            return DataTableToJObject(dataTable);
        }

        private static IEnumerable<JObject> ErrorDataForApprove(FileScheduler fileScheduler)
        {
            string errorMessage;
            var dbLayer = new ErrorDbLayer(fileScheduler.FileDetail.ErrorTable);
            var dataTable = dbLayer.GetDataForApprove(fileScheduler.Id, out errorMessage);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Log.Fatal(errorMessage);
                throw new Exception(errorMessage);
            }

            return DataTableToJObject(dataTable);
        }

        #endregion

        #region Edit Error Data

        public static void EditErrorData(bool validate, string fileAliasName, string tableName, Dictionary<string, object> errorData)
        {
            string errorMessage;

            var aliasName = (ColloSysEnums.FileAliasName)Enum.Parse(
                                typeof(ColloSysEnums.FileAliasName), fileAliasName);
            var datarow = DictionaryToDataRow(errorData);

            if (validate)
            {
                var isValid = SaveEditedErrorRecord.ValidateErrorRow(aliasName, datarow, out errorMessage);

                if (!isValid)
                {
                    Log.Fatal(errorMessage);
                    throw new Exception(errorMessage);
                }

                errorData[UploaderConstants.ErrorDescription] = string.Empty;
                errorData[UploaderConstants.ErrorStatus] = Enum.GetName(typeof(ColloSysEnums.ErrorStatus), ColloSysEnums.ErrorStatus.Submitted);
            }
            else
            {
                SaveEditedErrorRecord.ValidateErrorRow(aliasName, datarow, out errorMessage);
                if (string.IsNullOrWhiteSpace(errorMessage)) errorMessage = "Valid Record.";
                errorData[UploaderConstants.ErrorDescription] = errorMessage;
                errorData[UploaderConstants.ErrorStatus] = Enum.GetName(typeof(ColloSysEnums.ErrorStatus), ColloSysEnums.ErrorStatus.Edited);
            }

            var dbLayer = new ErrorDbLayer(tableName);
            dbLayer.UpdateErrorData(errorData, out errorMessage);


            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            Log.Fatal(errorMessage);
            throw new Exception(errorMessage);
        }

        public static void DeleteErrorData(string tableName, Guid id)
        {
            string errorMessage;
            var dbLayer = new ErrorDbLayer(tableName);
            dbLayer.DeleteData(id, out errorMessage);


            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            Log.Fatal(errorMessage);
            throw new Exception(errorMessage);
        }

        public static void ApproveErrorData(string fileAliasName, string tableName, Dictionary<string, object> errorData)
        {
            string errorMessage;

            var aliasName = (ColloSysEnums.FileAliasName)Enum.Parse(
                                typeof(ColloSysEnums.FileAliasName), fileAliasName);
            var datarow = DictionaryToDataRow(errorData);

            var isSaved = SaveEditedErrorRecord.SaveErrorRow(aliasName, datarow, out errorMessage);

            if (!isSaved)
            {
                Log.Fatal(errorMessage);
                throw new Exception(errorMessage);
            }

            errorData[UploaderConstants.ErrorDescription] = string.Empty;
            errorData[UploaderConstants.ErrorStatus] = ColloSysEnums.ErrorStatus.Approved;

            var dbLayer = new ErrorDbLayer(tableName);
            dbLayer.UpdateErrorData(errorData, out errorMessage);


            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            Log.Fatal(errorMessage);
            throw new Exception(errorMessage);
        }

        public static void RejectErrorData(string tableName, Dictionary<string, object> errorData)
        {
            string errorMessage;

            errorData[UploaderConstants.ErrorDescription] = string.Empty;
            errorData[UploaderConstants.ErrorStatus] = ColloSysEnums.ErrorStatus.Rejected;

            var dbLayer = new ErrorDbLayer(tableName);
            dbLayer.UpdateErrorData(errorData, out errorMessage);

            if (string.IsNullOrWhiteSpace(errorMessage))
                return;

            Log.Fatal(errorMessage);
            throw new Exception(errorMessage);
        }

        #endregion

        #region Retry error rows

        public static void RetryErrorRows(FileScheduler fileScheduler)
        {
            fileScheduler = GetFileScheduler(fileScheduler.Id);

            string errorMessage;
            var dbLayer = new ErrorDbLayer(fileScheduler.FileDetail.ErrorTable);
            var dataTable = dbLayer.GetDataFromSchedulerId(fileScheduler.Id, out errorMessage);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Log.Fatal(errorMessage);
                throw new Exception(errorMessage);
            }

            foreach (DataRow errorRow in dataTable.Rows)
            {
                SaveEditedErrorRecord.SaveErrorRow(fileScheduler.FileDetail.AliasName, errorRow, out errorMessage);

                errorRow[UploaderConstants.ErrorDescription] = errorMessage;
                errorRow[UploaderConstants.ErrorStatus] = string.IsNullOrWhiteSpace(errorMessage) ?
                                                                ColloSysEnums.ErrorStatus.RetrySuccess.ToString() : ColloSysEnums.ErrorStatus.DataError.ToString();

                var errorDictionary = errorRow.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => errorRow.Field<object>(col.ColumnName));

                dbLayer.UpdateErrorData(errorDictionary, out errorMessage);

                if (string.IsNullOrWhiteSpace(errorMessage))
                    continue;

                Log.Fatal(errorMessage);
                throw new Exception(errorMessage);
            }
        }

        #endregion

        #region Conversion

        private static IEnumerable<JObject> DataTableToJObject(DataTable dataTable)
        {
            return (from DataRow dr in dataTable.Rows
                    select dataTable.Columns
                                    .Cast<DataColumn>()
                                    .ToDictionary(col => col.ColumnName, col => dr[col])
                        into row
                        select JObject.FromObject(row))
                .ToList();
        }

        private static DataRow DictionaryToDataRow(Dictionary<string, object> dictionary)
        {
            var dt = new DataTable();

            dt.Columns.AddRange(dictionary.Select(d => new DataColumn(d.Key)).ToArray());

            var dataRow = dt.NewRow();

            foreach (var d in dictionary)
            {
                dataRow[d.Key] = d.Value;
            }

            return dataRow;
        }

        #endregion

        #region FileDetailServices

        private static FileScheduler GetFileScheduler(Guid id)
        {
            var session = SessionManager.GetCurrentSession();
            return (session.QueryOver<FileScheduler>().Fetch(x => x.FileDetail).Eager.Fetch(x => x.FileDetail.FileMappings).Eager.Where(c => c.Id == id).SingleOrDefault());
        }
        #endregion
    }
}