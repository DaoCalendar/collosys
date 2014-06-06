#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

#endregion


namespace ColloSys.FileUploadService.BaseReader
{
    public class SqlDataLayer
    {
        private readonly SqlConnection _con;

        public SqlDataLayer()
        {
            var connString = FileUploaderService.ConnString;
            _con = new SqlConnection(connString.ConnectionString);
        }

        public DataTable GetData(string query, out string errorMessage)
        {
            var dt = new DataTable();

            try
            {
                using (var da = new SqlDataAdapter(query, _con))
                {
                    da.Fill(dt);
                }
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            errorMessage = String.Empty;
            return dt;
        }

        public bool ExecuteNonQueryWithParam(string query, Dictionary<string, object> paramDictionary, out string errorMessage)
        {
            try
            {
                _con.Open();
                using (var cmd = new SqlCommand(query, _con))
                {
                    var sqlParams = paramDictionary.Select(d => new SqlParameter(d.Key, d.Value));
                    cmd.Parameters.AddRange(sqlParams.ToArray());
                    cmd.ExecuteNonQuery();
                    _con.Close();
                }
            }
            catch (SqlException ex)
            {
                _con.Close();
                errorMessage = ex.Message;
                return false;
            }

            errorMessage = String.Empty;
            return true;
        }

        public bool ExecuteNonQuery(string query, out string errorMessage)
        {
            try
            {
                _con.Open();
                using (var cmd = new SqlCommand(query, _con))
                {
                    cmd.ExecuteNonQuery();
                    _con.Close();
                }
            }
            catch (SqlException ex)
            {
                _con.Close();
                errorMessage = ex.Message;
                return false;
            }

            errorMessage = String.Empty;
            return true;
        }

        public static void ExecuteNonQuery(string createQuery)
        {
            using (var con = new SqlConnection(FileUploaderService.ConnString.ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand(createQuery, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}