#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploadService.BaseReader;
using ColloSys.Shared.ErrorTables;
using NLog;

#endregion

namespace ColloSys.FileUploadService.BaseReader
{
    public class ErrorDbLayer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        const string PrimaryKey = UploaderConstants.ErrorPrimaryKey;
        public readonly string TableName;
        private readonly SqlDataLayer _dataLayer;

        public ErrorDbLayer(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new InvalidOperationException("Table Name should not be empty or white space");
            }

            TableName = tableName;
            _dataLayer = new SqlDataLayer();
        }

        public DataTable GetDataFromSchedulerId(Guid fileSchedulerId, out string errorMessage)
        {
            var status = new List<string>
                {
                   "'"+ ColloSysEnums.ErrorStatus.DataError+"'",
                   "'"+ ColloSysEnums.ErrorStatus.Edited+"'",
                   "'"+ ColloSysEnums.ErrorStatus.Rejected+"'"
                };

            var query = string.Format("SELECT * FROM {0} WHERE {1} in ({2}) and {3} ='{4}'", TableName, UploaderConstants.ErrorStatus,
                                      string.Join(",", status), UploaderConstants.ErrorFileUploaderId, fileSchedulerId);

            var data = _dataLayer.GetData(query, out errorMessage);
            _logger.Info("DataError: Retreive error rows : " + data.Rows.Count);
            return data;
        }

        public DataTable GetData(out string errorMessage)
        {
            var status = new List<string>
                {
                   "'"+ ColloSysEnums.ErrorStatus.DataError+"'",
                   "'"+ ColloSysEnums.ErrorStatus.Edited+"'",
                   "'"+ ColloSysEnums.ErrorStatus.Rejected+"'"
                };

            var query = string.Format("SELECT * FROM {0} WHERE {1} in ({2})", TableName, UploaderConstants.ErrorStatus,
                                      string.Join(",", status));

            var data = _dataLayer.GetData(query, out errorMessage);
            _logger.Info("DataError: Retreive error rows : " + data.Rows.Count);
            return data;
        }

        public DataTable GetDataForApprove(Guid fileSchedulerId, out string errorMessage)
        {
            var query = string.Format("SELECT * FROM {0} WHERE {1}='{2}' and {3} ='{4}'", TableName, UploaderConstants.ErrorStatus,
                                      ColloSysEnums.ErrorStatus.Submitted, UploaderConstants.ErrorFileUploaderId, fileSchedulerId);

            var data = _dataLayer.GetData(query, out errorMessage);
            _logger.Info("DataError: Retreive error rows for approval : " + data.Rows.Count);
            return data;
        }



        public DataRow GetData(Guid rowId, out string errorMessage)
        {
            var query = string.Format("SELECT * FROM {0} WHERE {1}='{2}'", TableName,
                                        UploaderConstants.ErrorPrimaryKey, rowId);

            return _dataLayer.GetData(query, out errorMessage).Rows[0];
        }

        public bool UpdateErrorData(Dictionary<string, object> dictionary, out string errorMessage)
        {
            var emptyDic = dictionary.Where(x => x.Value == null || string.IsNullOrWhiteSpace(x.Value.ToString())).Select(x=>x.Key).ToList<string>();

            foreach(var e in emptyDic)
            {
                dictionary.Remove(e);
                dictionary.Add(e, string.Empty);
            }

            var colValues = dictionary.Select(d => string.Format("{0}=@{1}", d.Key, d.Key));

            string query = string.Format("UPDATE {0} SET {1} WHERE {2}='{3}'",
                                        TableName, string.Join(",", colValues),
                                        PrimaryKey, dictionary[PrimaryKey]);
            return _dataLayer.ExecuteNonQueryWithParam(query, dictionary, out errorMessage);
        }

        public bool DeleteData(Guid id, out string errorMessage)
        {
            string query = string.Format("UPDATE {0} SET {1}='{2}' WHERE {3}='{4}'", TableName, UploaderConstants.ErrorStatus,
                                                                                    ColloSysEnums.ErrorStatus.Ignore, PrimaryKey, id);

            return _dataLayer.ExecuteNonQuery(query, out errorMessage);
        }
    }
}
