using System.Collections.Generic;
using System.Data;

namespace ColloSys.UserInterface.Areas.Generic.Models
{
    public class QueryResult
    {
        public QueryResult()
        {
            Data = new DataTable();
            Error = string.Empty;
            if (Queries == null)
                Queries = new List<string>();
        }

        public DataTable Data { get; set; }

        public string Error { get; set; }

        public int RowsAffected { get; set; }

        public static IList<string> Queries { get; set; }

        public void AddQuery(string query)
        {
            if (!Queries.Contains(query.ToLowerInvariant().Trim()))
                Queries.Add(query.Trim());
        }
    }
}