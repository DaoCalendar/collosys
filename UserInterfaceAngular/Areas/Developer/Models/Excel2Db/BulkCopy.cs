#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

#endregion

namespace ColloSys.UserInterface.Areas.Developer.Models.Excel2Db
{
    class BulkCopy
    {
        #region upload data

        private readonly string _connectionString;

        public BulkCopy(string connString)
        {
            _connectionString = connString;
        }

        public void UploadData(DataSet data)
        {
            foreach (DataTable datatable in data.Tables)
            {
                DeleteEmptyRows(datatable);
                ChangeKey2Guid(datatable);
                UploadDataSql(datatable);
              
            }
        }

        #endregion

        #region hacks: empty rows & GUID Handling
        private void ChangeKey2Guid(DataTable datatable)
        {
            for (int index = 0; index < datatable.Columns.Count; index++)
            {
                if (datatable.Columns[index].ColumnName.EndsWith("@GUID_COLUMN"))
                {
                    string columnName = datatable.Columns[index].ColumnName.Replace("@GUID_COLUMN", string.Empty);
                    DataColumn column = new DataColumn(columnName, typeof(Guid));
                    datatable.Columns.Add(column);
                    datatable.AcceptChanges();

                    foreach (DataRow dr in datatable.Rows)
                    {
                        dr[datatable.Columns.Count - 1] = dr[index];
                    }
                    datatable.Columns.RemoveAt((int)index);
                    datatable.AcceptChanges();

                    datatable.Columns[datatable.Columns.Count - 1].SetOrdinal((int)index);
                    datatable.AcceptChanges();
                }
            }
        }

        private void DeleteEmptyRows(DataTable dataTable)
        {
            List<DataRow> deletedRows = new List<DataRow>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRow[0].ToString() == string.Empty)
                    deletedRows.Add(dataRow);
            }

            foreach (DataRow dataRow in deletedRows)
            {
                dataRow.Delete();
            }
            dataTable.AcceptChanges();
        }
        #endregion

        #region BulkCopy
        private void UploadDataSql(DataTable data)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(connection))
                {
                    // add column names
                    foreach (DataColumn column in data.Columns)
                    {
                        bulkcopy.ColumnMappings.Add(column.ColumnName.Trim(), column.ColumnName.Trim());
                    }

                    // add table name
                    bulkcopy.DestinationTableName = data.TableName;

                    // write to server
                    bulkcopy.BatchSize = 100;
                    bulkcopy.WriteToServer(data);

                    connection.Close();
                }
            }
        }

        //private void UploadDataOracle(DataTable data)
        //{
        //    using (var connection = new OracleConnection(_connectionString))
        //    {
        //        connection.Open();

        //        using (var bulkcopy = new OracleBulkCopy(connection))
        //        {
        //            bulkcopy.DestinationTableName = data.TableName;
        //            bulkcopy.BatchSize = 100;
        //            bulkcopy.WriteToServer(data);

        //            connection.Close();
        //        }
        //    }
        //}
        #endregion
    }
}
