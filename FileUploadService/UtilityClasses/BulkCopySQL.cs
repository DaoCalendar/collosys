#region references

using System.Data;
using System.Data.SqlClient;
using NLog;

#endregion

namespace ColloSys.FileUploadService.BaseReader
{
    public class BulkCopySql
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int BatchSize { get; set; }
        private int NotifyAfter { get; set; }
        private readonly string _connectionString;

        public BulkCopySql(string connection, int batchSize = 1000)
        {
            _connectionString = connection;
            BatchSize = batchSize;
            NotifyAfter = batchSize;
        }

        public void CopyDataIntoDbTable(DataTable table)
        {
            _logger.Info(string.Format("BulkCopy: copying data to table : {0}", table.TableName));
            using (var bulkCopy = new SqlBulkCopy(_connectionString))
            {
                bulkCopy.DestinationTableName = table.TableName;
                bulkCopy.BatchSize = BatchSize;
                bulkCopy.NotifyAfter = NotifyAfter;
                bulkCopy.WriteToServer(table);
            }
        }

        //public void CopyDataIntoDbTable(DataTable table, IEnumerable<string> columnName )
        //{
        //    _logger.Info(string.Format("BulkCopy: copying data to table : {0}", table.TableName));
        //    using (var bulkCopy = new SqlBulkCopy(_connectionString))
        //    {
        //        bulkCopy.DestinationTableName = table.TableName;
        //        foreach (var name in columnName)
        //        {
        //            bulkCopy.ColumnMappings.Add(name, name);
        //        }
        //        bulkCopy.BatchSize = BatchSize;
        //        bulkCopy.NotifyAfter = NotifyAfter;
        //        bulkCopy.WriteToServer(table);
        //    }
        //}
    }

}

