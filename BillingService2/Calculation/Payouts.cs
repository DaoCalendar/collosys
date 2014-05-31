using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService2.DBLayer;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.Test.QueryExecution;
using NLog;

namespace BillingService2.Calculation
{
    public class Payouts
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BillStatus _billStatus;
        private readonly List<BillingSubpolicy> _formulaList;
        private readonly List<BMatrix> _bMatrices;

        public Payouts(BillStatus billStatus)
        {
            _billStatus = billStatus;
            _formulaList = BillingSubpolicyDbLayer.GetFormulas(billStatus.Products);
            _bMatrices = BMatrixDbLayer.GetMatrices(billStatus.Products);
        }

        #region Payout

        public IList<BillDetail> ExecutePolicyOnLiner(List<DHFL_Liner> dhflLiners, ColloSysEnums.PolicyType policyType)
        {
            var billDetails = new List<BillDetail>();

            var billingPolicy = (BillingPolicyDbLayer.GetBillingPolicy(_billStatus.Stakeholder, policyType)
                                   ?? BillingPolicyDbLayer.GetBillingPolicy(_billStatus.Stakeholder.Hierarchy, policyType))
                                   ?? BillingPolicyDbLayer.GetBillingPolicy(_billStatus.Products, policyType);


            if (billingPolicy == null)
                return billDetails;

            var billingSubpolicies = BillingPolicyDbLayer.GetSubpolicies(billingPolicy, _billStatus.BillMonth);

            for (var i = 0; i < billingSubpolicies.Count; i++)
            {
                var billingSubpolicy = billingSubpolicies[i];

                billDetails.Add(GetBillDetail(billingPolicy, billingSubpolicy,
                                              dhflLiners));
            }

            return billDetails;
        }

        private BillDetail GetBillDetail(BillingPolicy billingPolicy, BillingSubpolicy billingSubpolicy, List<DHFL_Liner> dhflLiners)
        {
            var billDetail = new BillDetail
            {
                Id = Guid.NewGuid(),
                Stakeholder = _billStatus.Stakeholder,
                BillMonth = _billStatus.BillMonth,
                BillCycle = _billStatus.BillCycle,
                Products = _billStatus.Products,
                PaymentSource = ColloSysEnums.PaymentSource.Variable,
                BillingPolicy = billingPolicy,
                BillingSubpolicy = billingSubpolicy
            };

            List<DHFL_Liner> dhflLinersBillDeatail;
            var queryExecuter = new QueryExecuter<DHFL_Liner>(billingSubpolicy.BillTokens, _formulaList, _bMatrices);

            switch (billingPolicy.PolicyType)
            {
                case ColloSysEnums.PolicyType.Payout:
                    queryExecuter.ForEachFuction = ForEachFuctionPayout;
                    dhflLinersBillDeatail = dhflLiners
                                                .Where(x => x.BillStatus == ColloSysEnums.BillStatus.Unbilled && x.BillDetail == null)
                                                .ToList();
                    break;
                case ColloSysEnums.PolicyType.Capping:
                    queryExecuter.ForEachFuction = ForEachFuctionCapping;
                    dhflLinersBillDeatail = dhflLiners
                                               .Where(x => x.BillStatus == ColloSysEnums.BillStatus.PayoutApply)
                                               .ToList();
                    break;
                case ColloSysEnums.PolicyType.PF:
                    queryExecuter.ForEachFuction = ForEachFuctionPf;
                    dhflLinersBillDeatail = dhflLiners
                                               .Where(x => x.BillStatus == ColloSysEnums.BillStatus.PayoutApply)
                                               .ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(billingPolicy.PolicyType.ToString());
            }

            queryExecuter.ExeculteOnList(dhflLinersBillDeatail);

            if (billingPolicy.PolicyType == ColloSysEnums.PolicyType.Payout)
            {
                dhflLinersBillDeatail.ForEach(x => x.BillDetail =
                    (x.BillStatus == ColloSysEnums.BillStatus.PayoutApply)
                        ? billDetail
                        : null);
            }

            Logger.Info(string.Format("variable biling for stakeholder : {0}, product : {1}, subpolicy : {2} " +
                                           "and month : {3} has Amount : {4}", _billStatus.Stakeholder.Name,
                                          _billStatus.Products, billingSubpolicy.Name, _billStatus.BillMonth,
                                          billDetail.Amount));

            billDetail.Amount = dhflLiners.Sum(x => x.Payout);

            return billDetail;
        }

        private void ForEachFuctionPayout(DHFL_Liner dhtfLiner, decimal outputValue)
        {
            dhtfLiner.Payout = outputValue;
            dhtfLiner.BillStatus = ColloSysEnums.BillStatus.PayoutApply;
        }

        private void ForEachFuctionCapping(DHFL_Liner dhtfLiner, decimal outputValue)
        {
            dhtfLiner.Payout = outputValue;
            dhtfLiner.BillStatus = ColloSysEnums.BillStatus.CappingApply;
        }

        private void ForEachFuctionPf(DHFL_Liner dhtfLiner, decimal outputValue)
        {
            dhtfLiner.DeductPf = outputValue;
            dhtfLiner.BillStatus = ColloSysEnums.BillStatus.PfApply;
        }

        #endregion



        //public IList<BillDetail> GetAdhocPayout()
        //{
        //    var billDetails = new List<BillDetail>();

        //    var adhocPayments = BillAdhocDbLayer.GetBillAdhocForStkholder(_stakeholder, _billStatus.Products,
        //                                                                  _billStatus.BillMonth);
        //    for (var i = 0; i < adhocPayments.Count; i++)
        //    {
        //        var adhocPayment = adhocPayments[i];

        //        var billDetail = new BillDetail
        //        {
        //            Stakeholder = _stakeholder,
        //            BillMonth = _billStatus.BillMonth,
        //            BillCycle = _billStatus.BillCycle,
        //            Products = _billStatus.Products,
        //            PaymentSource = ColloSysEnums.PaymentSource.Adhoc,
        //            BillingPolicy = null,
        //            BillingSubpolicy = null,
        //            BillAdhoc = adhocPayment,
        //            Amount = (adhocPayment.IsCredit) ? (adhocPayment.TotalAmount / adhocPayment.Tenure) : (-1) * adhocPayment.TotalAmount
        //        };

        //        adhocPayment.RemainingAmount -= (adhocPayment.TotalAmount / adhocPayment.Tenure);
        //        billDetails.Add(billDetail);
        //    }

        //    BillAdhocDbLayer.SaveBillAdhoc(adhocPayments);

        //    return billDetails;
        //}

        //public BillDetail GetFixedPayout(StkhPayment stkhPayment)
        //{
        //    var billDetail = new BillDetail
        //    {
        //        Stakeholder = _stakeholder,
        //        BillMonth = _billStatus.BillMonth,
        //        BillCycle = _billStatus.BillCycle,
        //        Products = _billStatus.Products,
        //        Amount = stkhPayment.FixpayTotal + stkhPayment.MobileElig + stkhPayment.TravelElig,
        //        PaymentSource = ColloSysEnums.PaymentSource.Fixed,
        //        BillingPolicy = null,
        //        BillingSubpolicy = null
        //    };

        //    Logger.Info(string.Format("fixed biling for stakeholder : {0}, " +
        //                        "and product : {1}, and month {2} has amount : {3}",
        //                        _stakeholder.Name, _billStatus.Products,
        //                        _billStatus.BillMonth, billDetail.Amount));

        //    return billDetail;
        //}






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
