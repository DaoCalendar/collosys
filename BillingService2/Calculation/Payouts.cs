#region references

using System;
using System.Collections.Generic;
using System.Linq;
using BillingService2.DBLayer;
using BillingService2.QueryExecution;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using NLog;

#endregion

namespace BillingService2.Calculation
{
    public class Payouts
    {
        #region ctor
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BillStatus _billStatus;
        private readonly List<BillingSubpolicy> _formulaList;
        private readonly List<BMatrix> _bMatrices;
        private readonly BillingInfoManager _billingInfoManager;

        public Payouts(BillStatus billStatus)
        {
            _billStatus = billStatus;
            _formulaList = BillingSubpolicyDbLayer.GetFormulas(billStatus.Products);
            _bMatrices = BMatrixDbLayer.GetMatrices(billStatus.Products);
            _billingInfoManager = new BillingInfoManager();

        }
        #endregion

        #region Payout

        public IEnumerable<BillDetail> ExecutePolicyOnLiner(List<DHFL_Liner> dhflLiners, ColloSysEnums.PolicyType policyType)
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

        public List<DHFL_Info> GetDhflInfos()
        {
            return _billingInfoManager.InfoList.Select(x => x.Value).ToList();
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
                                                .Where(x => x.BillStatus == ColloSysEnums.BillStatus.Unbilled 
                                                    && x.BillDetail == null && !x.IsExcluded)
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
                                               .Where(x => x.BillStatus == ColloSysEnums.BillStatus.CappingApply
                                               && !x.IsExcluded)
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
                billDetail.Amount = dhflLinersBillDeatail.Sum(x => x.Payout);
            }
            else if (billingPolicy.PolicyType == ColloSysEnums.PolicyType.Capping)
            {
                billDetail.Amount = dhflLinersBillDeatail.Sum(x => x.DeductCap);
            }
            else if (billingPolicy.PolicyType == ColloSysEnums.PolicyType.PF)
            {
                billDetail.BaseAmount = dhflLinersBillDeatail.Where(x => x.ProratedProcFee > 0).Sum(x => x.SanAmt);
                billDetail.Amount = dhflLinersBillDeatail.Sum(x => x.ProratedProcFee);
            }

            Logger.Info(string.Format("variable biling for stakeholder : {0}, product : {1}, subpolicy : {2} " +
                                           "and month : {3} has Amount : {4}", _billStatus.Stakeholder.Name,
                                          _billStatus.Products, billingSubpolicy.Name, _billStatus.BillMonth,
                                          billDetail.Amount));

            billDetail.PolicyType = billingPolicy.PolicyType;
            billDetail.OriginMonth = _billStatus.OriginMonth;
            return billDetail;
        }
        #endregion

        #region for each
        private void ForEachFuctionPayout(DHFL_Liner liner, decimal outputValue)
        {
            _billingInfoManager.ManageInfoBeforePayout(liner);

            liner.Payout = outputValue;
            liner.BillStatus = ColloSysEnums.BillStatus.PayoutApply;

            _billingInfoManager.ManageInfoAfterPayout(liner);
        }

        private void ForEachFuctionCapping(DHFL_Liner liner, decimal outputValue)
        {
            _billingInfoManager.ManageInfoBeforeCapping(liner);

            var actualPayout = liner.Payout;
            liner.Payout = outputValue < 0 ? 0 : outputValue;
            liner.DeductCap = liner.Payout - actualPayout;
            liner.BillStatus = ColloSysEnums.BillStatus.CappingApply;

            _billingInfoManager.ManageInfoAfterCapping(liner);
        }

        private void ForEachFuctionPf(DHFL_Liner liner, decimal outputValue)
        {
            _billingInfoManager.ManageInfoBeforeProcFee(liner);

            var isSubventionCase = liner.Subvention.Trim().ToUpperInvariant() == "Y";
            var isNotFirstDisbursement = liner.TotalDisbAmt != 0;
            liner.ExpectedProcFee = outputValue;
            var percent = Decimal.Round((liner.ExpectedProcFee / (liner.SanAmt / 100)), 4);
            liner.ProratedProcFee = (isSubventionCase || isNotFirstDisbursement)
                ? 0 : Decimal.Round(liner.FeeReceived / percent, 2);

            liner.BillStatus = ColloSysEnums.BillStatus.PfApply;
            _billingInfoManager.ManageInfoAfterProcFee(liner);
        }
        #endregion
    }
}










































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
