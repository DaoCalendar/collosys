using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;

namespace BillingService2.Calculation
{
    public class ExpressionBuilder<T>
    {
        private BillDetail _billDetail;
        private List<BillTokens> _billTokenses;
        private List<T> _data;

        public ExpressionBuilder(BillDetail billDetail, List<BillTokens> billTokenses, List<T> data)
        {
            _billDetail = billDetail;
            _billTokenses = billTokenses;
            _data = data;
        }

        //public Expression<Func<T, bool>> GetConditionExpression()
        //{
        //    var parameter = Expression.Parameter(typeof(T), "x");
        //    Expression expression = null;

        //    for (var i = 0; i < _billTokenses.Count; i++)
        //    {
        //        var bCondition = _billTokenses[i];
        //        expression = (expression == null)
        //                         ? GetConditionExpression(parameter, billDetail, bCondition, data)
        //                         : Expression.AndAlso(expression, GetConditionExpression(parameter, billDetail, bCondition, data, traceLogs));
        //    }

        //    return Expression.Lambda<Func<T, bool>>(expression, parameter);
        //}

        internal List<List<BillTokens>> SplitBTokanWithAnd()
        {
            var andTokanList = new List<List<BillTokens>>();

            var list = new List<BillTokens>();
            for (int i = 0; i < _billTokenses.Count(); i++)
            {
                var billToken = _billTokenses[i];

                if (billToken.Text == "Operator" && billToken.Value == "And")
                {
                    andTokanList.Add(list);
                    list = new List<BillTokens>();
                    continue;
                }

                list.Add(billToken);
            }
            andTokanList.Add(list);

            return andTokanList;
        }

        public Expression<Func<T, bool>> GetAndConditionExpression()
        {
            

            //var e = DynamicExpression(new[] { p }, null, exp);

            var andTokanList = SplitBTokanWithAnd();
            
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression expression = null;
            for (int i = 0; i < andTokanList.Count; i++)
            {
                var tokanList = andTokanList[i];
                var conditionExpression = GetOperatorConditionExpression(parameter, tokanList);
                expression = (expression == null)
                                 ? conditionExpression
                                 : Expression.AndAlso(expression, conditionExpression);
            }

            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }





        internal static Expression GetAndConditionExpression(ParameterExpression parameter, BillTokens btTokens)
        {
            switch (btTokens.Type)
            {
                case "Column":
                    //return GetConditionExpression(parameter, billDetail, btTokens, data);
                case "Operator":


            }
        }

    }
}
