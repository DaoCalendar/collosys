using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService.ViewModel;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using NLog;

namespace BillingService.DBLayer
{
    internal static class CustBillViewModelDbLayer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // get amount based on subpolicy
        public static decimal GetBillingSubpolicyAmount<T>(ScbEnums.Products products, List<BCondition> bConditions, List<T> custBillViewModels)
        {
            var conditions = bConditions.Where(x => x.ConditionType == ColloSysEnums.ConditionType.Condition)
                                        .OrderBy(x => x.Priority);
            var output = bConditions.Where(x => x.ConditionType == ColloSysEnums.ConditionType.Output)
                                    .OrderBy(x => x.Priority);

            var expCondition = ExpressionBuilder.GetConditionExpression<T>(products, conditions.ToList(), custBillViewModels);

            var filterData = custBillViewModels.Where(expCondition.Compile()).ToList();

            if (filterData.Count <= 0)
                return 0;

            var result = ExpressionBuilder.GetOutputExpression(products, output.ToList(), filterData);

            Logger.Info(string.Format("Product : {0},Condition : {1} give value : {2}", products, expCondition.Body.ToString(), result));


            return Math.Round(result, 4);
        }
    }
}

//public static decimal GetBillingSubpolicyAmount<T>(ScbEnums.Products products, BillingSubpolicy billingSubpolicy, List<T> custBillViewModels)
//           where T : CustStkhBillViewModel
//       {
//           var bConditions = billingSubpolicy.BConditions;

//           var conditions = bConditions.Where(x => x.ConditionType == ColloSysEnums.ConditionType.Condition)
//                                       .OrderBy(x => x.Priority);
//           var output = bConditions.Where(x => x.ConditionType == ColloSysEnums.ConditionType.Output)
//                                   .OrderBy(x => x.Priority);

//           var expCondition = ExpressionBuilder.GetConditionExpression<T>(conditions.ToList());

//           var filterData = custBillViewModels.Where(expCondition.Compile()).ToList();

//           var amount = ExpressionBuilder.GetOutputExpression(products, output.ToList(), filterData);

//           return amount;
//       }
