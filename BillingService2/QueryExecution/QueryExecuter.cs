#region references

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using BillingService2.Calculation;
using BillingService2.DBLayer;
using BillingService2.ViewModel;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.QueryBuilder.Test.QueryExecution
{
    public class QueryExecuter<T> where T : class
    {
        #region ctor
        private readonly IList<BillTokens> _billTokenses;
        private readonly QueryGenerator<T> _stringQueryBuilder;
        private readonly List<BillingSubpolicy> _formulaList;
        private readonly IList<BMatrix> _bMatrices;

        public QueryExecuter(IList<BillTokens> billTokenses, List<BillingSubpolicy> formulaList = null, IList<BMatrix> bMatrices = null)
        {
            _billTokenses = billTokenses;
            _formulaList = formulaList ?? new List<BillingSubpolicy>();
            _bMatrices = bMatrices ?? new List<BMatrix>();
            _stringQueryBuilder = new QueryGenerator<T>(_formulaList);
        }
        #endregion

        #region executer

        public delegate void ForEachFuc(T obj, decimal value);

        public ForEachFuc ForEachFuction { get; set; }

        public List<T> ExeculteOnList(List<T> dataList)
        {
            var filterData = ConditionExecuter(dataList);
            var resultData = OutputExecuter(filterData);

            return resultData;
        }

        private List<T> ConditionExecuter(List<T> dataList)
        {
            var conditionToken = _billTokenses.Where(x => x.GroupType == "Condition").OrderBy(x => x.Priority).ToList();

            if (conditionToken.Count <= 0)
                return dataList;

            _stringQueryBuilder.DataList = dataList;
            var stringConditionQuery = _stringQueryBuilder.GenerateQuery(conditionToken);

            var conditionExpression = DynamicExpression.ParseLambda<T, bool>(stringConditionQuery);
            var resultData = dataList.Where(x => conditionExpression.Compile().Invoke(x)).ToList();
            return resultData;
        }

        private List<T> OutputExecuter(List<T> dataList)
        {
            var outPutToken = _billTokenses.Where(x => x.GroupType == "Output").OrderBy(x => x.Priority).ToList();

            if (outPutToken.Count <= 0)
                return dataList;

            if (ForEachFuction == null)
                throw new NotImplementedException("ForEachFunction not implimented");

            if (outPutToken[0].Type == "Formula" && outPutToken[0].DataType == "IfElse")
            {
                return FormulaExecuter(outPutToken[0].Value, dataList);
            }

            if (outPutToken[0].Type == "Matrix")
            {
                return MatrixExecuter(outPutToken[0].Value, dataList);
            }

            _stringQueryBuilder.DataList = dataList;
            var stringOutputQuery = _stringQueryBuilder.GenerateOutputQuery(outPutToken);
            var outputExpression = DynamicExpression.ParseLambda<T, decimal>(stringOutputQuery);
            //dataList.ForEach(x => x.Bucket = outputExpression.Compile().Invoke(x));

            dataList.ForEach(x => ForEachFuction(x, outputExpression.Compile().Invoke(x)));

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

                var queryExecuter = new QueryExecuter<T>(tokens, _formulaList, _bMatrices)
                {
                    ForEachFuction = ForEachFuction
                };
                queryExecuter.ExeculteOnList(dataList);
            }

            return dataList;
        }

        private List<T> MatrixExecuter(string matrixId, List<T> dataList)
        {
            if (dataList.Count <= 0)
                return dataList;

            var bMatrix = _bMatrices.SingleOrDefault(x => x.Id == Guid.Parse(matrixId));

            if (bMatrix == null)
                throw new ArgumentNullException(string.Format("matrixName : {0} not exist", matrixId));

            var matrixTokens = MatrixCalulater.GetBConditionsForMatrix(bMatrix);

            var groupIds = matrixTokens.Select(x => x.GroupId).Distinct().OrderBy(x => x).ToList();

            for (int i = 0; i < groupIds.Count; i++)
            {
                var groupId = groupIds[i];
                var tokens = matrixTokens.Where(x => x.GroupId == groupId).ToList();

                var queryExecuter = new QueryExecuter<T>(tokens, _formulaList, _bMatrices)
                {
                    ForEachFuction = ForEachFuction
                };
                queryExecuter.ExeculteOnList(dataList);
            }

            return dataList;
        }
        #endregion
    }
}