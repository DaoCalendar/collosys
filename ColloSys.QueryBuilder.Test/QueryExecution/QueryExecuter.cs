#region references

using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.QueryBuilder.Test.QueryExecution
{
    public class QueryExecuter<T> where T : CustBillViewModel
    {
        #region ctor
        private readonly IList<BillTokens> _billTokenses;
        private readonly QueryGenerator _stringQueryBuilder;

        public QueryExecuter(IList<BillTokens> billTokenses)
        {
            _billTokenses = billTokenses;
            _stringQueryBuilder = new QueryGenerator();
        }
        #endregion

        #region executer

        public List<T> ExeculteOnList(List<T> dataList)
        {
            var filterData = ConditionExecuter(dataList);
            var resultData = OutputExecuter(filterData);

            return resultData;
        }

        private List<T> ConditionExecuter(List<T> dataList)
        {
            var conditionToken = _billTokenses.Where(x => x.GroupType == "Condition").ToList();

            if (conditionToken.Count <= 0)
                return dataList;

            var stringConditionQuery = _stringQueryBuilder.GenerateAndOrQuery(conditionToken);

            var conditionExpression = DynamicExpression.ParseLambda<CustBillViewModel, bool>(stringConditionQuery);
            var resultData = dataList.Where(x => conditionExpression.Compile().Invoke(x)).ToList();
            return resultData;
        }

        private List<T> OutputExecuter(List<T> dataList)
        {
            var outPutToken = _billTokenses.Where(x => x.GroupType == "Output").ToList();

            if (outPutToken.Count <= 0)
                return dataList;

            if (outPutToken[0].Type == "Formula" && outPutToken[0].DataType == "IfElse")
            {
                return FormulaExecuter(dataList);
            }

            if (outPutToken[0].Type == "Matrix")
            {
                MatrixExecuter(dataList);
            }

            var stringOutputQuery = _stringQueryBuilder.GenerateOutputQuery(outPutToken);
            var outputExpression = DynamicExpression.ParseLambda<CustBillViewModel, uint>(stringOutputQuery);
            dataList.ForEach(x => x.Bucket = outputExpression.Compile().Invoke(x));

            return dataList;
        }

        private List<T> FormulaExecuter(List<T> dataList)
        {
            var formula = new BillingSubpolicy();

            var formulaTokens = formula.BillTokens;

            var groupIds = formulaTokens.Select(x => x.GroupId).Distinct().OrderBy(x => x).ToList();

            for (int i = 0; i < groupIds.Count; i++)
            {
                var groupId = groupIds[i];
                var tokens = formulaTokens.Where(x => x.GroupId == groupId).ToList();

                var queryExecuter = new QueryExecuter<T>(tokens);
                queryExecuter.ExeculteOnList(dataList);
            }

            return dataList;
        }

        private List<T> MatrixExecuter(string matrixName, List<T> dataList)
        {


            return new List<T>();
        }
        #endregion
    }
}