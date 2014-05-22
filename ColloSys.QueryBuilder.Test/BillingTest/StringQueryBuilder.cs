using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Billing;

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class StringQueryBuilder
    {
        private static bool OpenBracketAttached = false;
        private static bool OperatorAttached = false;
        private static BillTokens lastToken = new BillTokens();
        private static List<BillTokens> TokensList = new List<BillTokens>();

        public IList<BillTokens> City_CityCategory_Flag_Product_Tokens()
        {
            var query = new List<BillTokens>
            {
                new BillTokens {Type = "Table",Value = "CustBillViewModel.City",Priority = 0,DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 1, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Pune", Priority = 2, DataType = "string"},

                new BillTokens {Type = "Operator", Value = "AND", Priority = 3, DataType = "relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.CityCategory", Priority = 4, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 5, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "Tier1", Priority = 6, DataType = "string"},

                new BillTokens {Type = "Operator", Value = "AND", Priority = 7, DataType = "relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Flag", Priority = 8, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 9, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "O", Priority = 10, DataType = "string"},

                 new BillTokens {Type = "Operator", Value = "AND", Priority = 11, DataType = "relational"},

                new BillTokens {Type = "Table", Value = "CustBillViewModel.Product", Priority = 12, DataType = "string"},
                new BillTokens {Type = "Operator", Value = "EqualTo", Priority = 13, DataType = "conditional"},
                new BillTokens {Type = "Value", Value = "PL", Priority = 14, DataType = "string"}
            };
            return query;
        }

        public string GenerateQuery(IList<BillTokens> tokensList)
        {
            TokensList = tokensList.ToList();
            var query = string.Empty;

            foreach (var token in tokensList)
            {
                query = AttachTokenToString(query, token);
            }
            return query;
        }

        private string AttachTokenToString(string query, BillTokens token)
        {
            switch (token.Type)
            {
                case "Formula":
                case "Table":

                    query = Process_Formula_Table(query, token);
                    break;

                case "Operator":
                    query = Process_Operators(query, token);
                    break;

                case "Sql":
                    query = Process_SqlFunctions(query, token);
                    break;
                case "Value":
                    query = Process_Value(query, token);
                    break;
            }

            return query;
        }

        private string Process_Formula_Table(string query, BillTokens token)
        {
            if (lastToken.Type == "Formula" || lastToken.Type == "Table" || lastToken.Type == "Value")
                throw new Exception("Field can not attached to field");

            query += OpenBracketAttached ? token.Value : "(" + token.Value;
            OpenBracketAttached = true;
            SetLastToken(token);
            return query;
        }

        private string Process_Value(string query, BillTokens token)
        {
            if (lastToken.Type == "Formula" || lastToken.Type == "Table" || lastToken.Type == "Value")
                throw new Exception("value can not be attached to Formula/Table/Value");

            query += token.DataType == "string"
                ? "\"" + token.Value + "\""
                : token.Value;
            SetLastToken(token);
            query += IsLastToken(token) && OpenBracketAttached ? ")" : "";
            return query;
        }

        #region Operators

        private string Process_Operators(string query, BillTokens token)
        {
            switch (token.DataType)
            {
                case "conditional":
                    query = Process_Operators_Conditional(query, token);
                    break;
                case "relational":
                    query = Process_Operators_Relational(query, token);
                    break;
            }
            return query;
        }

        private string Process_Operators_Conditional(string query, BillTokens token)
        {
            if (lastToken.Type == "Operator" || lastToken.Type == "Sql")
                throw new Exception("Operator can not be attached to operator");
            query += Convert_Operator(token);
            SetLastToken(token);
            return query;
        }

        private string Process_Operators_Relational(string query, BillTokens token)
        {
            if (lastToken.Type == "Operator" || lastToken.Type == "Sql")
                throw new Exception("Operator can not be attached to operator");
            query += ") " + token.Value + "(";
            SetLastToken(token);
            OpenBracketAttached = true;
            return query;
        }

        #endregion

        #region supported function

        private string Process_SqlFunctions(string query, BillTokens token)
        {
            return query;
        }

        private void SetLastToken(BillTokens token)
        {
            lastToken = token;
        }

        private int Index(BillTokens token)
        {
            return TokensList.IndexOf(token);
        }

        private bool IsLastToken(BillTokens token)
        {
            var index = Index(token);
            return TokensList.Count - 1 == index;
        }

        private string Convert_Operator(BillTokens token)
        {
            if (token.Type != "Operator")
                throw new Exception("Non operator type can not be converted");
            switch (token.Value)
            {
                case "GreaterThan":
                    return ">";
                case "GreaterThanEqualTo":
                    return ">=";
                case "LessThan":
                    return "<";
                case "LessThanEqualTo":
                    return "<=";
                case "EqualTo":
                    return "=";
            }
            return token.Value;
        }

        #endregion
    }
}
