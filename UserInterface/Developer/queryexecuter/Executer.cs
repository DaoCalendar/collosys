#region references

using System;
using System.Data;
using System.Data.SqlClient;
using ColloSys.Shared.ConfigSectionReader;

#endregion

namespace ColloSys.UserInterface.Areas.Generic.Models
{
    public static class QueryExecuter
    {
        private static readonly string ConnString = ColloSysParam.WebParams.ConnectionString.ConnectionString;

        public static DataTable ExecuteNonScaler(string query2Execute)
        {
            if (string.IsNullOrEmpty(query2Execute))
            {
                return new DataTable();
            }
            var query = query2Execute;
            var dataTable = new DataTable();
            try
            {
                var conn = new SqlConnection(ConnString);
                var cmd = new SqlCommand(query, conn);
                conn.Open();

                // create data adapter
                var da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
            }
            catch (Exception ex)
            {
                dataTable = new DataTable();
                dataTable.Columns.Add("Exception");
                var dr = dataTable.NewRow();
                dr["Exception"] = ex;
                dataTable.Rows.Add(dr);
            }
            
            return dataTable;
        }

        public static int ExecuteNonQueryUpdateDelete(string query2Execute)
        {
            if (string.IsNullOrEmpty(query2Execute))
            {
                return 0;
            }
            
                var connection = new SqlConnection(ConnString);
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = query2Execute;
                var result = cmd.ExecuteNonQuery();
                connection.Close();
            
            return result;
        }

        public static int ExecuteScalerSelect(string query2Execute)
        {
            if (string.IsNullOrEmpty(query2Execute))
            {
                return 0;
            }
            var connection = new SqlConnection(ConnString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = query2Execute;

            var result = ((int)cmd.ExecuteScalar());
            connection.Close();
            return result;
        }
    }
}