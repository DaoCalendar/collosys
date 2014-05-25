using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;

namespace ColloSys.QueryBuilder.Test.BillingTest
{
    public class StringQueryBuilder
    {
        #region ctor

        private bool _openBracketAttached;
        private BillTokens _lastToken;
        private List<BillTokens> _tokensList = new List<BillTokens>();
        private readonly string _tableName = string.Empty;

        public StringQueryBuilder(string tableName = "")
        {
            _tableName = tableName;
        }

        #endregion

        public string GenerateQuery(IList<BillTokens> tokensList)
        {
            _tokensList = tokensList.ToList();
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
                default:
                    throw new ArgumentOutOfRangeException("token");
            }

            return query;
        }

        private string Process_Formula_Table(string query, BillTokens token)
        {
            if (_lastToken.GroupType == token.GroupType &&
                (_lastToken.Type == "Formula" || _lastToken.Type == "Table" || _lastToken.Type == "Value"))
                throw new Exception("Field can not attached to field");

            if (token.Type == "Table")
            {
                var seperatedList = token.Value.Split('.');
                token.Value = string.IsNullOrEmpty(_tableName)
                    ? seperatedList[1]
                    : _tableName + "." + seperatedList[1];
            }
            query += _openBracketAttached ? token.Value : "(" + token.Value;
            query += _lastToken.Type == "Sql" ? ")" : "";
            _openBracketAttached = true;
            SetLastToken(token);
            query += IsLastToken(token) && _openBracketAttached ? ")" : "";
            return query;
        }

        private string Process_Value(string query, BillTokens token)
        {
            if (_lastToken.Type == "Formula" || _lastToken.Type == "Table" || _lastToken.Type == "Value")
                throw new Exception("value can not be attached to Formula/Table/Value");

            switch (token.DataType)
            {
                case "string":
                    if (_openBracketAttached)
                    {
                        query += "\"" + token.Value + "\"";
                    }
                    else
                    {
                        query += "(\"" + token.Value + "\"";
                        _openBracketAttached = true;
                    }
                    break;
                default:
                    if (_openBracketAttached)
                    {
                        query += token.Value;
                    }
                    else
                    {
                        query += "(" + token.Value;
                        _openBracketAttached = true;
                    }
                    break;
            }
            //query +=OpenBracketAttached ? (token.DataType == "string"
            //    ? "\"" + token.Value + "\""
            //    : token.Value) : (token.DataType == "string"
            //    ? "(\"" + token.Value + "\""
            //    : "("+token.Value);
            SetLastToken(token);
            query += IsLastToken(token) && _openBracketAttached ? ")" : "";
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
                case "number":
                    query = Process_Operators_Conditional(query, token);
                    break;
            }
            return query;
        }

        private string Process_Operators_Conditional(string query, BillTokens token)
        {
            if (_lastToken.Type == "Operator" || _lastToken.Type == "Sql")
                throw new Exception("Operator can not be attached to operator");
            query += Convert_Operator(token);
            SetLastToken(token);
            return query;
        }

        private string Process_Operators_Relational(string query, BillTokens token)
        {
            if (_lastToken.Type == "Operator" || _lastToken.Type == "Sql")
                throw new Exception("Operator can not be attached to operator");
            query += ") " + token.Value + " (";
            SetLastToken(token);
            _openBracketAttached = true;
            return query;
        }

        #endregion

        #region supported function

        private string Process_SqlFunctions(string query, BillTokens token)
        {
            if (_lastToken.Type == "Formula" || _lastToken.Type == "Table" || _lastToken.Type == "Value")
                throw new Exception("value can not be attached to Formula/Table/Value");

            if (_openBracketAttached)
            {
                query += token.Value + "(";
            }
            else
            {
                query += "(" + token.Value + "(";
                _openBracketAttached = true;
            }
            SetLastToken(token);
            //query += IsLastToken(token) && _openBracketAttached ? ")" : "";
            return query;
        }

        private void SetLastToken(BillTokens token)
        {
            _lastToken = token;
        }

        private int Index(BillTokens token)
        {
            return _tokensList.IndexOf(token);
        }

        private bool IsLastToken(BillTokens token)
        {
            var index = Index(token);
            return _tokensList.Count - 1 == index;
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
                case "Plus":
                    return "+";
                case "Minus":
                    return "-";
                case "Divide":
                    return "/";
                case "Multiply":
                    return "*";
            }
            return token.Value;
        }

        #endregion
    }
}