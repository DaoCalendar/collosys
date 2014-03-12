#region references

using System.Collections.Generic;
using System.Linq;
using BillingService.CustBillView;
using BillingService.DBLayer;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using NLog;

#endregion


namespace BillingService.ViewModel
{
    public static class Payouts
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly BillAdhocBuilder BillAdhocBuilder=new BillAdhocBuilder();
        private static readonly BillingSubpolicyBuilder BillingSubpolicyBuilder=new BillingSubpolicyBuilder();

        #region Payout

        public static BillDetail GetFixedPayout(Stakeholders stakeholder, BillStatus billStatus, StkhPayment stkhPayment)
        {
            var billDetail = new BillDetail
            {
                Stakeholder = stakeholder,
                BillMonth = billStatus.BillMonth,
                BillCycle = billStatus.BillCycle,
                Products = billStatus.Products,
                Amount = stkhPayment.FixpayTotal,
                PaymentSource = ColloSysEnums.PaymentSource.Fixed,
                BillingPolicy = null,
                BillingSubpolicy = null
            };

            Logger.Info(string.Format("fixed biling for stakeholder : {0}, " +
                                "and product : {1}, and month {2} has amount : {3}",
                                stakeholder.Name, billStatus.Products,
                                billStatus.BillMonth, billDetail.Amount));

            return billDetail;
        }

        public static IEnumerable<BillDetail> GetVariablePayout(Stakeholders stakeholder, BillStatus billStatus, BillingPolicy billingPolicy, List<CustStkhBillViewModel> custStkhBillViewModels)
        {
            var billDetails = new List<BillDetail>();

            var billingSubpolicies =BillingSubpolicyBuilder.SubpolicyOnPolicy(billingPolicy, billStatus.BillMonth).ToList();
            for (var i = 0; i < billingSubpolicies.Count; i++)
            {
                var billingSubpolicy = billingSubpolicies[i];

                var billDetail = new BillDetail
                    {
                        Stakeholder = stakeholder,
                        BillMonth = billStatus.BillMonth,
                        BillCycle = billStatus.BillCycle,
                        Products = billStatus.Products,
                        PaymentSource = ColloSysEnums.PaymentSource.Variable,
                        BillingPolicy = billingPolicy,
                        BillingSubpolicy = billingSubpolicy,
                        Amount =
                            CustBillViewModelDbLayer.GetBillingSubpolicyAmount(billStatus.Products,
                                                                               billingSubpolicy.BConditions.ToList(),
                                                                               custStkhBillViewModels)
                    };
                
                //var test = custStkhBillViewModels.Sum(x => x.CustBillViewModel.TotalAmountRecovered);

                billDetails.Add(billDetail);

                Logger.Info(string.Format("variable biling for stakeholder : {0}, product : {1}, subpolicy : {2} " +
                                          "and month : {3} has Amount : {4}", stakeholder.Name,
                                          billStatus.Products, billingSubpolicy.Name, billStatus.BillMonth,
                                          billDetail.Amount));
            }

            return billDetails;
        }

        public static IEnumerable<BillDetail> GetAdhocPayout(Stakeholders stakeholder, BillStatus billStatus)
        {
            var billDetails = new List<BillDetail>();

            var adhocPayments = BillAdhocBuilder.ForStakeholder(stakeholder, billStatus.Products,
                                                                          billStatus.BillMonth).ToList();
            for (var i = 0; i < adhocPayments.Count; i++)
            {
                var adhocPayment = adhocPayments[i];

                var billDetail = new BillDetail
                {
                    Stakeholder = stakeholder,
                    BillMonth = billStatus.BillMonth,
                    BillCycle = billStatus.BillCycle,
                    Products = billStatus.Products,
                    PaymentSource = ColloSysEnums.PaymentSource.Adhoc,
                    BillingPolicy = null,
                    BillingSubpolicy = null,
                    BillAdhoc = adhocPayment,
                    Amount = (adhocPayment.IsCredit) ? (adhocPayment.TotalAmount / adhocPayment.Tenure) : (-1) * adhocPayment.TotalAmount
                };

                adhocPayment.RemainingAmount -= (adhocPayment.TotalAmount / adhocPayment.Tenure);
                billDetails.Add(billDetail);
            }
            BillAdhocBuilder.Save(adhocPayments);

            return billDetails;
        }
        #endregion


        #region Helper
        
        private static List<CustStkhBillViewModel> GetCustStkhBillViewModel(ScbEnums.Products products, bool isInRecovery)
        {
            var custBillViewModels = ProcessCustBillView.GetBillingServiceData(products, isInRecovery)
                //.Where(x => x.Stakeholders.Id == stakeholder.Id)
                                                          .ToList();

            var StkhBillViewModels = from c in custBillViewModels
                                     group c by c.Stakeholders
                                         into g
                                         select new StkhBillViewModel()
                                             {
                                                 Stakeholders = g.Key,
                                                 TotalAmountRecovered = g.Sum(x => x.TotalAmountRecovered),
                                                 TotalAmountAllocated = g.Sum(x => x.TotalDueOnAllocation),

                                                 Bucket1ResolutionPer = g.Any(x => x.Bucket == 1)
                                                                            ? (g.Where(x => x.Bucket == 1)
                                                                                .Sum(x => x.TotalAmountRecovered) * 100) /
                                                                              g.Where(x => x.Bucket == 1)
                                                                               .Sum(x => x.TotalDueOnAllocation)
                                                                            : 0,

                                                 Bucket2ResolutionPer = g.Any(x => x.Bucket == 2)
                                                                            ? (g.Where(x => x.Bucket == 2)
                                                                                .Sum(x => x.TotalAmountRecovered) * 100) /
                                                                              g.Where(x => x.Bucket == 2)
                                                                               .Sum(x => x.TotalDueOnAllocation)
                                                                            : 0,

                                                 Bucket3ResolutionPer = g.Any(x => x.Bucket == 3)
                                                                            ? (g.Where(x => x.Bucket == 3)
                                                                                .Sum(x => x.TotalAmountRecovered) * 100) /
                                                                              g.Where(x => x.Bucket == 3)
                                                                               .Sum(x => x.TotalDueOnAllocation)
                                                                            : 0,

                                                 Bucket4ResolutionPer = g.Any(x => x.Bucket == 4)
                                                                            ? (g.Where(x => x.Bucket == 4)
                                                                                .Sum(x => x.TotalAmountRecovered) * 100) /
                                                                              g.Where(x => x.Bucket == 4)
                                                                               .Sum(x => x.TotalDueOnAllocation)
                                                                            : 0,

                                                 Bucket5ResolutionPer = g.Any(x => x.Bucket == 5)
                                                                            ? (g.Where(x => x.Bucket == 5)
                                                                                .Sum(x => x.TotalAmountRecovered) * 100) /
                                                                              g.Where(x => x.Bucket == 5)
                                                                               .Sum(x => x.TotalDueOnAllocation)
                                                                            : 0,

                                                 Bucket6ResolutionPer = g.Any(x => x.Bucket == 6)
                                                                            ? (g.Where(x => x.Bucket == 6)
                                                                                .Sum(x => x.TotalAmountRecovered) * 100) /
                                                                              g.Where(x => x.Bucket == 6)
                                                                               .Sum(x => x.TotalDueOnAllocation)
                                                                            : 0,
                                             };

            var CustStkhBillViewModels = from c in custBillViewModels
                                         select new CustStkhBillViewModel()
                                         {
                                             CustBillViewModel = c,
                                             GPincode = c.GPincode,
                                             StkhBillViewModel =
                                                 StkhBillViewModels.SingleOrDefault(
                                                     x => x.Stakeholders.Id == c.Stakeholders.Id)
                                         };

            return CustStkhBillViewModels.ToList();
        }

        #endregion
    }
}
