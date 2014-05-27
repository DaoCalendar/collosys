#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using BillingService2.ViewModel;
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
        private readonly IList<BillingSubpolicy> _formulaList;
        private readonly IList<BMatrix> _bMatrices;

        public QueryExecuter(IList<BillTokens> billTokenses, IList<BillingSubpolicy> formulaList = null, IList<BMatrix> bMatrices = null)
        {
            _billTokenses = billTokenses;
            _stringQueryBuilder = new QueryGenerator();
            _formulaList = formulaList ?? new List<BillingSubpolicy>();
            _bMatrices = bMatrices ?? new List<BMatrix>();
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
                return FormulaExecuter(outPutToken[0].Value, dataList);
            }

            if (outPutToken[0].Type == "Matrix")
            {
                MatrixExecuter(dataList);
            }

            var stringOutputQuery = _stringQueryBuilder.GenerateOutputQuery(outPutToken);
            var outputExpression = DynamicExpression.ParseLambda<CustBillViewModel, dynamic>(stringOutputQuery);
            dataList.ForEach(x => x.Bucket = outputExpression.Compile().Invoke(x));

            return dataList;
        }

        private List<T> FormulaExecuter(string formulaName, List<T> dataList)
        {
            var formula = _formulaList.SingleOrDefault(x => x.Name == formulaName);

            if (formula == null)
                throw new Exception(string.Format("Formula : {0} not exist", formulaName));

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

        private List<T> MatrixExecuter(List<T> dataList)
        {
            var bMatrix=new BMatrix();

            var tokens = MatrixCalulater.GetBConditionsForMatrix(bMatrix);

            var queryExecuter = new QueryExecuter<T>(tokens);
            queryExecuter.ExeculteOnList(dataList);

            return new List<T>();
        }
        #endregion
    }
}