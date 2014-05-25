#region references

using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using ColloSys.DataLayer.Billing;
using ColloSys.QueryBuilder.Test.BillingTest;

#endregion

namespace ColloSys.QueryBuilder.Test.QueryExecution
{
    public class QueryExecuter<T> where T : CustBillViewModel
    {
        private readonly IList<BillTokens> _billTokenses;
        private readonly QueryGenerator _stringQueryBuilder;

        public QueryExecuter(IList<BillTokens> billTokenses)
        {
            _billTokenses = billTokenses;
            _stringQueryBuilder = new QueryGenerator();
        }

        public List<T> ConditionExecuter(List<T> dataList)
        {
            var conditionToken = _billTokenses.Where(x => x.GroupType == "Condition").ToList();

            if (conditionToken.Count <= 0)
                return dataList;

            var stringConditionQuery = _stringQueryBuilder.GenerateAndOrQuery(conditionToken);

            var conditionExpression = DynamicExpression.ParseLambda<CustBillViewModel, bool>(stringConditionQuery);
            var resultData = dataList.Where(x => conditionExpression.Compile().Invoke(x)).ToList();
            return resultData;
        }

        public List<T> OutputExecuter(List<T> dataList)
        {
            var outPutToken = _billTokenses.Where(x => x.GroupType == "Output").ToList();

            if (outPutToken.Count <= 0)
                return dataList;

            var stringOutputQuery = _stringQueryBuilder.GenerateOutputQuery(outPutToken);
            var outputExpression = DynamicExpression.ParseLambda<CustBillViewModel, uint>(stringOutputQuery);
            dataList.ForEach(x => x.Bucket = outputExpression.Compile().Invoke(x));

            return dataList;
        }


        public List<T> ExeculteOnList(List<T> dataList)
        {
            var filterData = ConditionExecuter(dataList);
            var resultData = OutputExecuter(filterData);

            return resultData;
        }

    }
}