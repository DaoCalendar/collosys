using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService2.DBLayer;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.Test.QueryExecution;
using NLog;

namespace BillingService2.Calculation
{
    public class Payouts
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Stakeholders _stakeholder;
        private readonly BillStatus _billStatus;

        public Payouts(Stakeholders stakeholder, BillStatus billStatus)
        {
            _stakeholder = stakeholder;
            _billStatus = billStatus;
        }

        #region Payout


        public IList<BillDetail> GetAdhocPayout()
        {
            var billDetails = new List<BillDetail>();

            var adhocPayments = BillAdhocDbLayer.GetBillAdhocForStkholder(_stakeholder, _billStatus.Products,
                                                                          _billStatus.BillMonth);
            for (var i = 0; i < adhocPayments.Count; i++)
            {
                var adhocPayment = adhocPayments[i];

                var billDetail = new BillDetail
                {
                    Stakeholder = _stakeholder,
                    BillMonth = _billStatus.BillMonth,
                    BillCycle = _billStatus.BillCycle,
                    Products = _billStatus.Products,
                    PaymentSource = ColloSysEnums.PaymentSource.Adhoc,
                    BillingPolicy = null,
                    BillingSubpolicy = null,
                    BillAdhoc = adhocPayment,
                    Amount = (adhocPayment.IsCredit) ? (adhocPayment.TotalAmount / adhocPayment.Tenure) : (-1) * adhocPayment.TotalAmount
                };

                adhocPayment.RemainingAmount -= (adhocPayment.TotalAmount / adhocPayment.Tenure);
                billDetails.Add(billDetail);
            }

            BillAdhocDbLayer.SaveBillAdhoc(adhocPayments);

            return billDetails;
        }

        public BillDetail GetFixedPayout(StkhPayment stkhPayment)
        {
            var billDetail = new BillDetail
            {
                Stakeholder = _stakeholder,
                BillMonth = _billStatus.BillMonth,
                BillCycle = _billStatus.BillCycle,
                Products = _billStatus.Products,
                Amount = stkhPayment.FixpayTotal + stkhPayment.MobileElig + stkhPayment.TravelElig,
                PaymentSource = ColloSysEnums.PaymentSource.Fixed,
                BillingPolicy = null,
                BillingSubpolicy = null
            };

            Logger.Info(string.Format("fixed biling for stakeholder : {0}, " +
                                "and product : {1}, and month {2} has amount : {3}",
                                _stakeholder.Name, _billStatus.Products,
                                _billStatus.BillMonth, billDetail.Amount));

            return billDetail;
        }

        public IList<BillDetail> GetVariablePayout(BillingPolicy billingPolicy, List<CustBillViewModel> custBillViewModels)
        {
            var billDetails = new List<BillDetail>();

            var billingSubpolicies = BillingPolicyDbLayer.GetSubpolicies(billingPolicy, _billStatus.BillMonth);

            for (var i = 0; i < billingSubpolicies.Count; i++)
            {
                var billingSubpolicy = billingSubpolicies[i];

                billDetails.Add(GetBillDetail(billingPolicy, billingSubpolicy,
                                              custBillViewModels));
            }

            return billDetails;
        }

        private BillDetail GetBillDetail(BillingPolicy billingPolicy, BillingSubpolicy billingSubpolicy, List<CustBillViewModel> custBillViewModels)
        {
            var billDetail = new BillDetail
            {
                Id = Guid.NewGuid(),
                Stakeholder = _stakeholder,
                BillMonth = _billStatus.BillMonth,
                BillCycle = _billStatus.BillCycle,
                Products = _billStatus.Products,
                PaymentSource = ColloSysEnums.PaymentSource.Variable,
                BillingPolicy = billingPolicy,
                BillingSubpolicy = billingSubpolicy
            };

            //TODO:Done please check 4 lines
            var custBillViewModelsNonBillDeatail = custBillViewModels;//.Where(x => x.BillDetail == null).ToList();

            // billDetail.Amount = 
            //GetBillingSubpolicyAmount(billDetail, billingSubpolicy.BillTokens.ToList(),
            //                                                                   custBillViewModelsNonBillDeatail);
            //billDetail.TraceLog = tracelog.GetLog();

            var queryExecuter = new QueryExecuter<CustBillViewModel>(billingSubpolicy.BillTokens);
            queryExecuter.ExeculteOnList(custBillViewModelsNonBillDeatail);

            Logger.Info(string.Format("variable biling for stakeholder : {0}, product : {1}, subpolicy : {2} " +
                                          "and month : {3} has Amount : {4}", _stakeholder.Name,
                                          _billStatus.Products, billingSubpolicy.Name, _billStatus.BillMonth,
                                          billDetail.Amount));

            return billDetail;
        }

        #endregion

        //#region Calculate Bill Amount

        //public static void GetBillingSubpolicyAmount<T>(BillDetail billDetail, List<BillTokens> billTokenses, List<T> custBillViewModels)
        //{
        //    var conditions = billTokenses.Where(x => x.GroupId == "0.Condition")
        //                                .OrderBy(x => x.Priority);
        //    var output = billTokenses.Where(x => x.GroupId == "0.Output")
        //                            .OrderBy(x => x.Priority);

        //    var expCondition = ExpressionBuilder.GetConditionExpression<T>(billDetail, conditions.ToList(), custBillViewModels, traceLogs);

        //    var filterData = custBillViewModels.Where(expCondition.Compile()).ToList();

        //    if (filterData.Count <= 0)
        //        return;

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
        //    //traceLogs.SetLog(log);

        //    Logger.Info(log);

        //    //return Math.Round(result, 4);
        //}


        //#endregion

    }
}
