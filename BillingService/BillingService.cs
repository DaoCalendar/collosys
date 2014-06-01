#region refernces

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using BillingService.CustBillView;
using BillingService.DBLayer;
using BillingService.MoveProductToPayment;
using BillingService.ViewModel;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.NhSetup;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.Shared.ConfigSectionReader;
using NLog;

#endregion


namespace ColloSys.BillingService
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
                    Logger.Info(string.Format("Moving Product from Info to Payment table started"));
                    MoveProductInfoToPayment.Init();
                    Logger.Info(string.Format("Moving Product from Info to Payment table Ended"));
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

            var custBillViewModels = ProcessCustBillView.GetBillingServiceData(billStatus.Products,
                                                                                  billStatus.BillMonth);

            Logger.Info(string.Format("Total {0} custstkBillViewModel available for product : {1}", custBillViewModels.Count(),
                                      billStatus.Products));

            var stakeholders = custBillViewModels.Select(x => x.Stakeholders).Distinct().ToList();
            // StakeholderDbLayer.GetStakeholdersForBilling(billStatus.Products, billStatus.BillMonth);

            Logger.Info(string.Format("Total {0} stakeholder available for product : {1}", stakeholders.Count(),
                                      billStatus.Products));

            var billDetails = new List<BillDetail>();
            foreach (var stakeholder in stakeholders)
            {
                var custBillViewModelsForStkholder = custBillViewModels
                    .Where(x => x.Stakeholders.Id == stakeholder.Id).ToList();

                billDetails.AddRange(BillingForStakeholder(stakeholder, billStatus, custBillViewModelsForStkholder));
            }

            BillStatusDbLayer.SaveDoneBillStatus(billStatus);
        }

        public IList<BillDetail> BillingForStakeholder(Stakeholders stakeholder, BillStatus billStatus, List<CustBillViewModel> custBillViewModels)
        {
            Logger.Info(string.Format("biling start for stakeholder : {0}, and product : {1}, and month {2}", stakeholder.Name,
                                      billStatus.Products, billStatus.BillMonth));

            var billDetails = new List<BillDetail>();

            var stkhPayment = stakeholder.StkhPayments.FirstOrDefault(x => x.Products == billStatus.Products);
            if (stkhPayment == null)
            {
                Logger.Info(string.Format("No working for stakeholder : {0} and product : {1}", stakeholder.Name,
                                          billStatus.Products));
                return billDetails;
            }

            // for fixed payment
            if (stakeholder.Hierarchy.HasFixed)
            {
                billDetails.Add(Payouts.GetFixedPayout(stakeholder, billStatus, stkhPayment));
            }

            // for variable payment
            if (stakeholder.Hierarchy.HasVarible)
            {
                // for collection
                var custBillViewModelsForCollection = custBillViewModels
                                                                .Where(x => !x.IsInRecovery)
                                                                .ToList();
                var collectionbillingPolicy = BillingPolicyDbLayer.GetPolicies(billStatus.Products, ScbEnums.Category.Liner);

                if (collectionbillingPolicy != null)
                    billDetails.AddRange(Payouts.GetVariablePayout(stakeholder, billStatus,
                                                                   collectionbillingPolicy, custBillViewModelsForCollection));


                // for recovery
                var custBillViewModelsForRecovery = custBillViewModels
                                                                .Where(x => x.IsInRecovery)
                                                                .ToList();
                var recoverybillingPolicy = BillingPolicyDbLayer.GetPolicies(billStatus.Products, ScbEnums.Category.WriteOff);

                if (recoverybillingPolicy != null)
                    billDetails.AddRange(Payouts.GetVariablePayout(stakeholder, billStatus,
                                                                   recoverybillingPolicy, custBillViewModelsForRecovery));
            }

            // for adhoc payment 
            billDetails.AddRange(Payouts.GetAdhocPayout(stakeholder, billStatus));

            var billAmount = GetBillAmountForStkholder(stakeholder, billStatus, billDetails);

            //ToDO:Done Please Check 2 lines
            var custBillViewModelsWithBillDetail = custBillViewModels.Where(x => x.BillDetail != null).ToList();
            BillDetailDbLayer.SaveBillDetailsBillAmount(billDetails, billAmount, custBillViewModelsWithBillDetail);

            return billDetails;
        }

        private BillAmount GetBillAmountForStkholder(Stakeholders stakeholders, BillStatus billStatus, List<BillDetail> billDetails)
        {
            var billAmount = new BillAmount();

            billAmount.Stakeholder = stakeholders;
            billAmount.Products = billStatus.Products;
            billAmount.BillMonth = billStatus.BillMonth;
            billAmount.Cycle = 0;
            billAmount.StartDate = DateTime.ParseExact(string.Format("{0}01", billAmount.BillMonth), "yyyyMMdd",
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
