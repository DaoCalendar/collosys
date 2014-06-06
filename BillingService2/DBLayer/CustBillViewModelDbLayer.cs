using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SessionMgr;

namespace BillingService2.DBLayer
{
    internal static class CustBillViewModelDbLayer
    {   
        public static List<CustBillViewModel> GetCustBillViewModelDbData(ScbEnums.Products products, uint billMonth)
        {
            var session = SessionManager.GetCurrentSession();
            var billingViewModel = session.QueryOver<CustBillViewModel>().List<CustBillViewModel>();

            return billingViewModel.ToList();
        }
    }
}


// private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
// get amount based on subpolicy
//public static decimal GetBillingSubpolicyAmount<T>(BillDetail billDetail, List<BCondition> bConditions, List<T> custBillViewModels, TraceLogs traceLogs)
//{
//    var conditions = bConditions.Where(x => x.ConditionType == ColloSysEnums.ConditionType.Condition)
//                                .OrderBy(x => x.Priority);
//    var output = bConditions.Where(x => x.ConditionType == ColloSysEnums.ConditionType.Output)
//                            .OrderBy(x => x.Priority);

//    var expCondition = ExpressionBuilder.GetConditionExpression<T>(billDetail, conditions.ToList(), custBillViewModels, traceLogs);

//    var filterData = custBillViewModels.Where(expCondition.Compile()).ToList();

//    if (filterData.Count <= 0)
//        return 0;

//    var result = ExpressionBuilder.GetOutputExpression(billDetail, output.ToList(), filterData, traceLogs);

//    foreach (var data in filterData)
//    {
//        traceLogs.AddCondition(expCondition.ToString());

//        var conditionSatify = data.GetType().GetProperty("ConditionSatisfy");
//        var oldValue = conditionSatify.GetValue(data);
//        conditionSatify.SetValue(data, traceLogs.GetConditionMatrixFormulaLog());


//        var billDetailPro = data.GetType().GetProperty("BillDetail");
//        billDetailPro.SetValue(data, billDetail);
//    }

//    var log = string.Format("Product : {0},Condition : {1} give value : {2}", billDetail.Products,
//                            expCondition.Body, result);
//    traceLogs.SetLog(log);

//    Logger.Info(log);

//    return Math.Round(result, 4);
//}






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
