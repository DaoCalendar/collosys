#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.SharedUtils;
using NLog;

#endregion

namespace ColloSys.Shared.ErrorTables
{
    public class CreateErrorTable
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public string CreateTableQueryByFileDetail(FileDetail fileDetail)
        {
            _logger.Info(string.Format("CreateTable: creating table from file details: {0} ", fileDetail.AliasName.ConvertToString()));
            var columns = new List<string>
                {
                    string.Format("[{0}] [uniqueidentifier] NOT NULL", UploaderConstants.ErrorPrimaryKey)
                };
            columns.AddRange(fileDetail.FileColumns.OrderBy(c => c.Position)
                                       .Select(c => string.Format("[{0}] [nvarchar](max) NULL", c.TempColumnName)));



            var res = fileDetail.FileColumns.SingleOrDefault(c => c.TempColumnName == UploaderConstants.ErrorFileDate);
            if (res == null)
                columns.Add(string.Format("[{0}] [nvarchar](max) NULL", UploaderConstants.ErrorFileDate));

            columns.Add(string.Format("[{0}] [nvarchar](max) NULL", UploaderConstants.ErrorDescription));
            columns.Add(string.Format("[{0}] [nvarchar](max) NULL", UploaderConstants.ErrorStatus));
            columns.Add(string.Format("[{0}] [nvarchar](max) NULL", UploaderConstants.ErrorFileRowNo));
            columns.Add(string.Format("[{0}] [uniqueidentifier] NOT NULL", UploaderConstants.ErrorFileUploaderId));

            var query = string.Format(
                "IF  NOT EXISTS (SELECT * FROM sys.objects " +
                " WHERE object_id = OBJECT_ID(N'{0}') AND type in (N'U'))" +
                "BEGIN " +
                " CREATE TABLE [dbo].[{0}](" +
                "{1}, CONSTRAINT [PK_{2}_{3}] PRIMARY KEY CLUSTERED" +
                "([{3}] ASC)" +
                "WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]" +
                ") ON [PRIMARY];" +
                "END"
                , fileDetail.ErrorTable, string.Join(",", columns), fileDetail.ErrorTable, UploaderConstants.ErrorPrimaryKey);

            return query;
        }
    }
}


        //public void CreateTableIfNotExist(FileDetail fileDetail)
        //{
        //    SqlQueryExecuter.ExecuteNonQuery(CreateTableQueryByFileDetail(fileDetail));
        //}

        //private DataTable _dt;
        //public void CreateTableIfNotExist(DataTable dataTable)
        //{
        //    _logger.Info(string.Format("CreateTable: creating table {0}", dataTable.TableName));
        //    _dt = dataTable;
        //    SqlDataLayer.ExecuteNonQuery(GenerateCreateTableQuery());
        //}

        //private string GenerateCreateTableQuery()
        //{
        //    var columns = new List<string>();

        //    foreach (DataColumn col in _dt.Columns)
        //    {
        //        if (col.DataType == typeof(Guid))
        //        {
        //            columns.Add(string.Format("[{0}] [uniqueidentifier] NOT NULL", col.ColumnName));
        //            continue;
        //        }

        //        columns.Add(string.Format("[{0}] [nvarchar](max) NULL", col.ColumnName));
        //    }

        //    return GenerateCreateTableQuery(columns);
        //}

        //private string GenerateCreateTableQuery(IEnumerable<string> columns)
        //{
        //    var query = string.Format(
        //        "IF  NOT EXISTS (SELECT * FROM sys.objects " +
        //        " WHERE object_id = OBJECT_ID(N'{0}') AND type in (N'U'))" +
        //        "BEGIN " +
        //        " CREATE TABLE [dbo].[{0}](" +
        //        "{1}, CONSTRAINT [PK_{2}_{3}] PRIMARY KEY CLUSTERED" +
        //        "([{3}] ASC)" +
        //        "WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]" +
        //        ") ON [PRIMARY];" +
        //        "END"
        //        , _dt.TableName, string.Join(",", columns), _dt.TableName, UploaderConstants.ErrorPrimaryKey);

        //    return query;
        //}