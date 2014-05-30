using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingService2.Calculation;
using BillingService2.DBLayer;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

namespace BillingService2
{
    public class BillingServices
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region nh init

        public static readonly ConnectionStringSettings ConnString;
        static BillingServices()
        {
            try
            {
                ConnString = ColloSysParam.WebParams.ConnectionString;
                Logger.Info(string.Format("BillingService: Connection String : {0}", ConnString.ConnectionString));
                SessionManager.InitNhibernate(new NhInitParams
                {
                    ConnectionString = ConnString,
                    DbType = ConfiguredDbTypes.MsSql,
                    IsWeb = false
                });
            }
            catch (Exception ex)
            {
                Logger.Error("BillingService : " + ex);
            }
        }

        #endregion


        public void StartBillingService()
        {
            try
            {
                SessionManager.BindNewSession();

                try
                {
                    //Logger.Info(string.Format("Moving Product from Info to Payment table started"));
                    //MoveProductInfoToPayment.Init();
                    //Logger.Info(string.Format("Moving Product from Info to Payment table Ended"));
                }
                catch (Exception exception)
                {
                    Logger.Info(string.Format("Error in moving product from Info to Payment table {0}", exception.Message));
                }

                var billStatuss = BillStatusDbLayer.GetPendingBillStatus();

                if (!billStatuss.Any())
                {
                    Logger.Info(string.Format("No product Pending for billing"));
                    return;
                }

                Logger.Info(string.Format("Total {0} product Pending for Billing", billStatuss.Count));
                foreach (var billStatus in billStatuss)
                {
                    BillingForBillStatus(billStatus);
                }
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format("Error : StartBillingService() : {0}", ex.Message));
            }
        }

        private void BillingForBillStatus(BillStatus billStatus)
        {
            Logger.Info(string.Format("biling start for month : {0}, and product : {1}", billStatus.BillMonth,
                                      billStatus.Products));

            var dhflLiners = DhflLinerDbLayer.GetDhflLinerForStkholderDbData(billStatus);

            Logger.Info(string.Format("Total {0} dhflLiners available for product : {1}, month : {2}, stakeholder : {3}", dhflLiners.Count(),
                                      billStatus.Products, billStatus.BillMonth, billStatus.Stakeholder.Name));

           

            var billDetails = new List<BillDetail>();
            var payouts = new Payouts(billStatus);


            billDetails.AddRange(payouts.ExecutePolicyOnLiner(dhflLiners,ColloSysEnums.PolicyType.Payout));

            billDetails.AddRange(payouts.ExecutePolicyOnLiner(dhflLiners, ColloSysEnums.PolicyType.Capping));

            billDetails.AddRange(payouts.ExecutePolicyOnLiner(dhflLiners, ColloSysEnums.PolicyType.PF));


            var billAmount = GetBillAmountForStkholder(billStatus, billDetails);


            BillStatusDbLayer.SaveDoneBillStatus(billStatus);
        }

        private BillAmount GetBillAmountForStkholder(BillStatus billStatus, List<BillDetail> billDetails)
        {
            var billAmount = new BillAmount();

            billAmount.Stakeholder = billStatus.Stakeholder;
            billAmount.Products = billStatus.Products;
            billAmount.Month = billStatus.BillMonth;
            billAmount.Cycle = 0;
            billAmount.StartDate = DateTime.ParseExact(string.Format("{0}01", billAmount.Month), "yyyyMMdd",
                                                CultureInfo.InvariantCulture);
            billAmount.EndDate = billAmount.StartDate.AddMonths(1).AddDays(-1);
            billAmount.Status = ColloSysEnums.ApproveStatus.Submitted;

            billAmount.FixedAmount = billDetails.Where(x => x.BillingPolicy == null
                                                            && x.BillingSubpolicy == null
                                                            && x.BillAdhoc == null)
                                                .Sum(x => x.Amount);

            billAmount.VariableAmount = Math.Round(billDetails.Where(x => x.BillingPolicy != null
                                                               && x.BillingSubpolicy != null
                                                               && x.BillAdhoc == null)
                                                   .Sum(x => x.Amount), 2);

            billAmount.Deductions = Math.Round(billDetails.Where(x => x.BillingPolicy == null
                                                           && x.BillingSubpolicy == null
                                                           && x.BillAdhoc != null)
                                               .Sum(x => x.Amount), 2);

            //ToDO:Done Please check 1 line
            var subTotal = billAmount.FixedAmount + billAmount.VariableAmount + billAmount.Deductions;

            // TODO: ICICI demo
            billAmount.HoldAmount = Math.Round(subTotal * Convert.ToDecimal(0.10));
            billAmount.HoldRepayment = 1000;
            billAmount.TotalAmount = (subTotal - billAmount.HoldAmount) + billAmount.HoldRepayment;

            billAmount.PayStatus = ColloSysEnums.BillPaymentStatus.BillingDone;
            billAmount.PayStatusDate = DateTime.Now;
            billAmount.PayStatusHistory = string.Format("{{ PayStatus: {0}, Date: {1} }}", billAmount.PayStatus, billAmount.PayStatusDate);

            return billAmount;
        }

        #region Helper

        //private static List<CustStkhBillViewModel> GetCustStkhBillViewModel(ScbEnums.Products products, uint billMonth)
        //{
        //    var custBillViewModels = ProcessCustBillView.GetBillingServiceData(products, billMonth);

        //    var CustStkhBillViewModels = from c in custBillViewModels
        //                                 select new CustStkhBillViewModel()
        //                                 {
        //                                     CustBillViewModel = c,
        //                                     GPincode = c.GPincode
        //                                 };

        //    return CustStkhBillViewModels.ToList();
        //}

        #endregion
    }
}


//public IList<BillDetail> BillingForStakeholder(Stakeholders stakeholder, BillStatus billStatus, List<DHFL_Liner> dhflLiners)
//        {
//            Logger.Info(string.Format("biling start for stakeholder : {0}, and product : {1}, and month {2}", stakeholder.Name,
//                                      billStatus.Products, billStatus.BillMonth));

//            var billDetails = new List<BillDetail>();

//            var stkhPayment = stakeholder.StkhPayments.FirstOrDefault(x => x.Products == billStatus.Products);
//            if (stkhPayment == null)
//            {
//                Logger.Info(string.Format("No working for stakeholder : {0} and product : {1}", stakeholder.Name,
//                                          billStatus.Products));
//                return billDetails;
//            }

//            var payouts = new Payouts(stakeholder, billStatus);

//            //// for fixed payment
//            //if (stakeholder.Hierarchy.HasFixed)
//            //{
//            //    billDetails.Add(payouts.GetFixedPayout(stkhPayment));
//            //}

//            // for variable payment
//            //if (stakeholder.Hierarchy.HasVarible)
//            //{
//            //    var collectionbillingPolicy = BillingPolicyDbLayer.GetPolicies(billStatus.Products, ScbEnums.Category.Liner);

//            //    if (collectionbillingPolicy != null)
//            //        billDetails.AddRange(payouts.GetVariablePayout(collectionbillingPolicy, dhflLiners));
//            //}

//            //// for adhoc payment 
//            //billDetails.AddRange(payouts.GetAdhocPayout());

//            var billingPolicy = (BillingPolicyDbLayer.GetPolicies(stakeholder)
//                                    ?? BillingPolicyDbLayer.GetPolicies(stakeholder.Hierarchy))
//                                    ?? BillingPolicyDbLayer.GetPolicies(billStatus.Products);


//            if (billingPolicy != null)
//                billDetails.AddRange(payouts.GetVariablePayout(billingPolicy, dhflLiners));


//            var billAmount = GetBillAmountForStkholder(stakeholder, billStatus, billDetails);

//            //ToDO:Done Please Check 2 lines
//            //var custBillViewModelsWithBillDetail = dhflLiners.Where(x => x.BillDetail != null).ToList();
//            //BillDetailDbLayer.SaveBillDetailsBillAmount(billDetails, billAmount, custBillViewModelsWithBillDetail);

//            return billDetails;
//        }