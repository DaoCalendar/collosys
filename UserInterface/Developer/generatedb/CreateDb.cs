﻿#region references

using System;
using System.Data.SqlClient;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Migrate;
using ColloSys.Shared.ConfigSectionReader;
using ColloSys.Shared.ErrorTables;
using ColloSys.UserInterface.Areas.Developer.Models.Excel2Db;
using ColloSys.UserInterface.Areas.Generic.Models;
using NHibernate.Tool.hbm2ddl;

#endregion

namespace ColloSys.UserInterface.Areas.Developer.Models
{
    public static class CreateDb
    {
        public static bool CreateDatabse()
        {
            if (!DeleteAllTables())
                return false;

            try
            {
                GenerateNewDb();
                MigrateToLatest();
                UploadExcelData();
                CreatErrortable();
                InitialData.InsertData();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void MigrateToLatest()
        {
            var createTable = MigrateRunner.VersionInfoTableCreate();
            QueryExecuter.ExecuteNonQueryUpdateDelete(createTable);
            var connString = ColloSysParam.WebParams.ConnectionString.ConnectionString;
            var insertInto = MigrateRunner.VersionInfoTableInsert(connString);
            if(string.IsNullOrWhiteSpace(insertInto)) return;
            QueryExecuter.ExecuteNonQueryUpdateDelete(insertInto);
        }

        private static void GenerateNewDb()
        {
            var schemaExport = new SchemaExport(SessionManager.GetNhConfiguration());
            schemaExport
                //.SetOutputFile(@"d:\db.sql")
               .Execute(false, true, false);
        }

        private static void UploadExcelData()
        {
            var uploader = new Excel2Db.Excel2Db(ColloSysParam.WebParams.ConnectionString.ConnectionString);
            uploader.UploadExcel2Db("FileUploader.xls");
        }

        private static void CreatErrortable()
        {
            var session = SessionManager.GetCurrentSession();
            var result = session.QueryOver<FileDetail>().List();
            var createErrorTable = new CreateErrorTable();
            foreach (var item in result)
            {
                if (item.ScbSystems == ScbEnums.ScbSystems.CCMS || item.ScbSystems == ScbEnums.ScbSystems.CACS)
                    continue;
                var query = createErrorTable.CreateTableQueryByFileDetail(item);
                var con = new SqlConnection(ColloSysParam.WebParams.ConnectionString.ConnectionString);
                con.Open();
                var cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        private static bool DeleteAllTables()
        {
            var connectionstring = ColloSysParam.WebParams.ConnectionString.ConnectionString;
            var connection = new SqlConnection(connectionstring);
            connection.Open();
            const string cmdText = "exec sp_MSforeachtable 'DROP TABLE ?'";
            var command = new SqlCommand(cmdText, connection);
            var i = 0;
            while (i != -1)
            {
                try
                {
                    i = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    // ReSharper disable RedundantJumpStatement
                    continue;
                    // ReSharper restore RedundantJumpStatement
                }
            }
            return true;
        }

    }
}