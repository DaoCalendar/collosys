#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.QueryBuilder.Test.QueryExecution
{
    public class QueryGenerator<T>
    {
        private readonly List<BillingSubpolicy> _formulaList;
        //private readonly List<T> _dataList; 
        public QueryGenerator(List<BillingSubpolicy> formulaList = null)
        {
            _formulaList = formulaList;
        }

        #region combine tokens
        public string GenerateOutputQuery(IEnumerable<BillTokens> tokensList)
        {
            return GenerateMathQuery(tokensList);
        }

        private string GenerateMathQuery(IEnumerable<BillTokens> tokens)
        {
            var query = string.Empty;
            foreach (var token in tokens)
            {
                string tokenString;
                switch (token.Type)
                {
                    case "Formula":
                        tokenString = ProcessFormula(token);
                        break;
                    case "Table":
                        tokenString = ProcessTableColumn(token);
                        break;
                    case "Operator":
                        tokenString = ProcessOperators(token);
                        break;
                    case "Sql":
                        tokenString = ProcessSqlFunctions(token);
                        break;
                    case "Value":
                        tokenString = ProcessValue(token);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(token.Type);
                }
                query += tokenString;
            }
            return query;
        }

        private string GenerateConditionalQuery(List<BillTokens> tokensList)
        {
            var conditionToken = tokensList.First(x => x.Type == "Operator" && x.DataType == "conditional");
            var index = tokensList.IndexOf(conditionToken);
            var lhsList = tokensList.Skip(0).Take(index).ToList();
            var rhsList = tokensList.Skip(index + 1).ToList();

            var lhsQuery = GenerateMathQuery(lhsList);
            var rhsQuery = GenerateMathQuery(rhsList);
            var oper = ProcessOperators(conditionToken);

            return string.Format("({0}) {1} ({2})", lhsQuery, oper, rhsQuery);
        }

        public string GenerateAndOrQuery(List<BillTokens> tokensList)
        {
            var query = string.Empty;
            var remainingTokens = tokensList;
            var conditionToken = tokensList.FirstOrDefault(x => x.Type == "Operator" && x.DataType == "relational");

            do
            {
                var condtionIndex = conditionToken == null
                    ? -1
                    : remainingTokens.IndexOf(conditionToken);
                var lhsTokens = conditionToken == null
                    ? remainingTokens.Skip(0).ToList()
                    : remainingTokens.Skip(0).Take(condtionIndex).ToList();

                query += string.Format("( {0} )", GenerateConditionalQuery(lhsTokens));
                if (conditionToken != null) query += ProcessOperators(conditionToken);

                remainingTokens = conditionToken == null
                    ? new List<BillTokens>()
                    : remainingTokens.Skip(condtionIndex + 1).ToList();
                conditionToken = remainingTokens.Count > 0
                    ? remainingTokens.FirstOrDefault(x => x.Type == "Operator" && x.DataType == "relational")
                    : null;
            } while (remainingTokens.Count > 0);

            return query;
        }

        public string GenerateQuery(List<BillTokens> tokensList)
        {
            if (tokensList[0].Type.ToLower() == "sql")
            {
                return ProcessSqlFunctions(tokensList);
            }

            return GenerateAndOrQuery(tokensList);
        }
        #endregion

        public List<T> DataList { get; set; }  

        #region operator tokens
        public string ProcessSqlFunctions(List<BillTokens> tokensList)
        {
            if (DataList == null || DataList.Count <= 0)
                return "0";

            var conditionExpression = DynamicExpression.ParseLambda<T, decimal>(ProcessTableColumn(tokensList[1]));

            switch (tokensList[0].Value.ToLower())
            {
                case "sum":
                    return DataList.Sum(x => conditionExpression.Compile().Invoke(x))
                                    .ToString(CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentOutOfRangeException(tokensList[0].Value);
            }
        }

        private string ProcessOperators(BillTokens token)
        {
            switch (token.DataType)
            {
                case "conditional":
                    return ProcessConditionalOperators(token);
                case "relational":
                    return ProcessRelationalOperators(token);
                case "number":
                    return ProcessNumericOperators(token);
                default:
                    throw new ArgumentOutOfRangeException(token.DataType);
            }
        }

        private string ProcessNumericOperators(BillTokens token)
        {
            switch (token.Value)
            {
                case "Plus":
                    return "+";
                case "Minus":
                    return "-";
                case "Divide":
                    return "/";
                case "Multiply":
                    return "*";
                default:
                    throw new ArgumentOutOfRangeException(token.Value);
            }
        }

        private string ProcessRelationalOperators(BillTokens token)
        {
            switch (token.Value.ToLower())
            {
                case "and":
                    return "&&";
                case "or":
                    return "||";
                default:
                    throw new ArgumentOutOfRangeException(token.Value);
            }
        }

        private string ProcessConditionalOperators(BillTokens token)
        {
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
                case "NotEqualTo":
                    return "!=";
                default:
                    throw new ArgumentOutOfRangeException(token.Value);
            }
        }
        #endregion

        #region non-operator tokens
        private string ProcessTableColumn(BillTokens token)
        {
            return token.Value.Replace("CustBillViewModel.", "").Replace("DhflLiner.", "");
        }

        //TODO: fix formula
        private string ProcessFormula(BillTokens token)
        {
            var formula = _formulaList.SingleOrDefault(x => x.Name == token.Value);

            if (formula == null)
                throw new ArgumentNullException(string.Format("Formula Name : {0} not found", token.Value));

            var tokens = formula.BillTokens.OrderBy(x=>x.Priority).ToList();
            var queryGenerator = new QueryGenerator<T>(_formulaList);
            queryGenerator.DataList = DataList;
            var formulaString = queryGenerator.GenerateQuery(tokens);


            return formulaString;
        }

        private string ProcessValue(BillTokens token)
        {
            decimal value;
            return decimal.TryParse(token.Value, out value) ? token.Value : string.Format("\"{0}\"", token.Value);


            //return (token.DataType == "string" || token.DataType == "Value" || token.DataType == "enum")
            //    ? string.Format("\"{0}\"", token.Value)
            //    : token.Value;
        }

        private string ProcessSqlFunctions(BillTokens token)
        {
            return token.Value;
        }
        #endregion
    }
}