#region references

using System.Data;
using System.Data.SqlClient;
using ColloSys.Shared.ConfigSectionReader;

#endregion

namespace AngularUI.Developer.queryexecuter
{
    public static class QueryExecuter
    {
        private static readonly string ConnString = ColloSysParam.WebParams.ConnectionString.ConnectionString;

        public static DataTable ExecuteSelect(string query2Execute)
        {
            if (string.IsNullOrEmpty(query2Execute))
            {
                return new DataTable();
            }
            var query = query2Execute;
            var dataTable = new DataTable();
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }
                conn.Close();
            }

            return dataTable;
        }

        public static int ExecuteDataChange(string query2Execute)
        {
            var result = 0;
            if (string.IsNullOrEmpty(query2Execute))
            {
                return result;
            }

            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query2Execute, conn))
                {
                    result = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return result;
        }
    }
}