#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BillingService.Utility;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.ClientDataBuilder;
using NHibernate;
using NHibernate.SqlCommand;

#endregion


namespace BillingService.CustBillView
{
    public class ProcessCustBillView
    {
        private static readonly PaymentBuilder PaymentBuilder=new PaymentBuilder();
        private static readonly AllocBuilder AllocBuilder=new AllocBuilder();



        public static IEnumerable<CustBillViewModel> GetBillingServiceData(ScbEnums.Products products, bool isInRecovery)
        {
            var billingViewModel = new List<CustBillViewModel>();


            var cAllocations = AllocBuilder.ForBilling(products, isInRecovery);
            var cPayments = PaymentBuilder.GetAll().ToList();
            billingViewModel.AddRange(cAllocations
                                          .Select(allocation =>
                                                  CreateCustBillViewModel(allocation, allocation.Info, cPayments))
                                          .ToList());
            return billingViewModel;
        }

        public static List<CustBillViewModel> GetBillingServiceData(ScbEnums.Products products, uint billMonth)
        {
            var billingViewModel = new List<CustBillViewModel>();

            DateTime startDate = DateTime.ParseExact(string.Format("{0}01", billMonth), "yyyyMMdd",
                                                CultureInfo.InvariantCulture); ;
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var cAllocations = AllocBuilder.ForBilling(products, startDate, endDate).ToList();
            var cPayments = PaymentBuilder.GetAll().ToList();
            billingViewModel.AddRange(cAllocations
                                          .Select(allocation =>
                                                  CreateCustBillViewModel(allocation, allocation.Info, cPayments))
                                          .ToList());

           

            return billingViewModel;
        }

        public static CustBillViewModel CreateCustBillViewModel(Allocations allocation, Info info, IList<Payment> payments)
        {
            var custBilViewModel = new CustBillViewModel
                {
                    AccountNo = info.AccountNo,
                    GlobalCustId = info.GlobalCustId,
                    Flag = info.Flag,
                    Product = info.Product,
                    Cycle = info.Cycle,
                    Bucket = (uint) allocation.Bucket,
                    IsInRecovery = info.IsInRecovery,
                    IsXHoldAccount = info.IsXHoldAccount,
                    CityCategory = info.GPincode.CityCategory,
                    AllocationStartDate = allocation.StartDate,
                    AllocationEndDate = allocation.EndDate,
                    TotalDueOnAllocation = allocation.AmountDue,
                    MobWriteoff = Util.GetMobWriteoff(info.ChargeofDate)
                };

            custBilViewModel.Vintage = Util.GetVintage(custBilViewModel.MobWriteoff, custBilViewModel.Product);

            var debitAmount = payments.Where(x => x.AccountNo == custBilViewModel.AccountNo
                                                  && (x.TransDate >= custBilViewModel.AllocationStartDate.Date
                                                      && x.TransDate <= custBilViewModel.AllocationEndDate.Value.Date)
                                                  && (x.IsDebit && !x.IsExcluded))
                                      .Sum(x => x.TransAmount);

            var creditAmount = payments.Where(x => x.AccountNo == custBilViewModel.AccountNo
                                                   && (x.TransDate >= custBilViewModel.AllocationStartDate.Date
                                                       && x.TransDate <= custBilViewModel.AllocationEndDate.Value.Date)
                                                   && (!x.IsDebit && !x.IsExcluded))
                                       .Sum(x => x.TransAmount);

            custBilViewModel.TotalAmountRecovered = debitAmount - creditAmount;

            custBilViewModel.ResolutionPercentage = (custBilViewModel.TotalDueOnAllocation == 0) ?
                0 : (custBilViewModel.TotalAmountRecovered * 100) / custBilViewModel.TotalDueOnAllocation;

            custBilViewModel.GPincode = info.GPincode;
            custBilViewModel.Stakeholders = allocation.Stakeholder;

            return custBilViewModel;
        }
    }
}

//public static List<CustBillViewModel> GetBillingServiceData(ScbEnums.Products products)
//{
//    var billingViewModel = new List<CustBillViewModel>();
//    var allocations = AllocBuilder.ForBilling(products);
//    var payments = PaymentBuilder.GetAll();
//    billingViewModel.AddRange(allocations.Select(alloc => CreateCustBillViewModel(alloc, alloc.Info, payments)));
//    return billingViewModel;
//}

//switch (systemOnProduct)
//{
//    case ScbEnums.ScbSystems.CCMS:
//        var cAllocations = GetAllocations(products, startDate, endDate, session);
//        var cPayments = GetPayment<Payment>(session);
//        billingViewModel.AddRange(cAllocations
//                                      .Select(allocation =>
//                                              CreateCustBillViewModel(allocation, allocation.Info, cPayments))
//                                      .ToList());
//        break;

//    case ScbEnums.ScbSystems.EBBS:
//        var eAllocations = GetEAllocations(products, startDate, endDate, session);
//        var ePayments = GetPayment<Payment>(session);
//        billingViewModel.AddRange(eAllocations
//                                      .Select(allocation =>
//                                              CreateCustBillViewModel(allocation, allocation.Info, ePayments))
//                                      .ToList());
//        break;

//    case ScbEnums.ScbSystems.RLS:
//        var rAllocations = GetRAllocations(products, startDate, endDate, session);
//        var rPayments = GetPayment<Payment>(session);
//        billingViewModel.AddRange(rAllocations
//                                      .Select(allocation =>
//                                              CreateCustBillViewModel(allocation, allocation.Info, rPayments))
//                                      .ToList());
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}


//switch (systemOnProduct)
//{
//    case ScbEnums.ScbSystems.CCMS:
//        var cAllocations = GetCAllocations(products, session);
//        var cPayments = GetPayment<Payment>(session);
//        billingViewModel.AddRange(cAllocations
//                                      .Select(allocation =>
//                                              CreateCustBillViewModel(allocation, allocation.Info, cPayments))
//                                      .ToList());
//        break;

//    case ScbEnums.ScbSystems.EBBS:
//        var eAllocations = GetAllocations(products, session);
//        var ePayments = GetPayment<Payment>(session);
//        billingViewModel.AddRange(eAllocations
//                                      .Select(allocation =>
//                                              CreateCustBillViewModel(allocation, allocation.Info, ePayments))
//                                      .ToList());
//        break;

//    case ScbEnums.ScbSystems.RLS:
//        var rAllocations = GetRAllocations(products, session);
//        var rPayments = GetPayment<Payment>(session);
//        billingViewModel.AddRange(rAllocations
//                                      .Select(allocation =>
//                                              CreateCustBillViewModel(allocation, allocation.Info, rPayments))
//                                      .ToList());
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}


//public static IList<EAlloc> GetEAllocations(ScbEnums.Products products, DateTime startDate, DateTime endDate, ISession session)
//{
//    EAlloc alloc = null;
//    EInfo info = null;
//    GPincode pincode = null;
//    Stakeholders stakeholders = null;
//    var eAllocations = session.QueryOver<EAlloc>(() => alloc)
//                            .Fetch(x => x.EInfo).Eager
//                            .Fetch(x => x.EInfo.GPincode).Eager
//                            .Fetch(x => x.Stakeholder).Eager
//                            .JoinQueryOver(() => alloc.EInfo, () => info, JoinType.InnerJoin)
//                            .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
//                            .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
//                            .Where(() => info.Product == products)
//                            .And(() => info.AllocStartDate >= startDate && info.AllocEndDate <= endDate)
//                            .List<EAlloc>();
//    return eAllocations;
//}

//public static IList<RAlloc> GetRAllocations(ScbEnums.Products products, DateTime startDate, DateTime endDate, ISession session)
//{
//    RAlloc alloc = null;
//    RInfo info = null;
//    GPincode pincode = null;
//    Stakeholders stakeholders = null;
//    var rAllocations = session.QueryOver<RAlloc>(() => alloc)
//                            .Fetch(x => x.RInfo).Eager
//                            .Fetch(x => x.RInfo.GPincode).Eager
//                            .Fetch(x => x.Stakeholder).Eager
//                            .JoinQueryOver(() => alloc.RInfo, () => info, JoinType.InnerJoin)
//                            .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
//                            .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
//                            .Where(() => info.Product == products)
//                            .And(() => info.AllocStartDate >= startDate && info.AllocEndDate <= endDate)
//                            .List<RAlloc>();
//    return rAllocations;
//}



//public static IList<RAlloc> GetRAllocations(ScbEnums.Products products, ISession session)
//{
//    RAlloc alloc = null;
//    RInfo info = null;
//    GPincode pincode = null;
//    Stakeholders stakeholders = null;
//    var rAllocations = session.QueryOver<RAlloc>(() => alloc)
//                            .Fetch(x => x.RInfo).Eager
//                            .Fetch(x => x.RInfo.GPincode).Eager
//                            .Fetch(x => x.Stakeholder).Eager
//                            .JoinQueryOver(() => alloc.RInfo, () => info, JoinType.InnerJoin)
//                            .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
//                            .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
//                            .Where(() => info.Product == products)
//                            .List<RAlloc>();
//    return rAllocations;
//}

//public static IList<CAlloc> GetCAllocations(ScbEnums.Products products, ISession session)
//{
//    CAlloc alloc = null;
//    CInfo info = null;
//    GPincode pincode = null;
//    Stakeholders stakeholders = null;
//    var cAllocations = session.QueryOver<CAlloc>(() => alloc)
//                            .Fetch(x => x.CInfo).Eager
//                             .Fetch(x => x.CInfo.GPincode).Eager
//                            .Fetch(x => x.Stakeholder).Eager
//                            .JoinQueryOver(() => alloc.CInfo, () => info, JoinType.InnerJoin)
//                            .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
//                            .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
//                            .Where(() => info.Product == products)
//                            .List<CAlloc>();
//    return cAllocations;
//}



//public static IList<RAlloc> GetRAllocations(ScbEnums.Products products, bool isInRecovery, ISession session)
//{
//    RAlloc alloc = null;
//    RInfo info = null;
//    GPincode pincode = null;
//    Stakeholders stakeholders = null;
//    var rAllocations = session.QueryOver<RAlloc>(() => alloc)
//                            .Fetch(x => x.RInfo).Eager
//                            .Fetch(x => x.RInfo.GPincode).Eager
//                            .Fetch(x => x.Stakeholder).Eager
//                            .JoinQueryOver(() => alloc.RInfo, () => info, JoinType.InnerJoin)
//                            .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
//                            .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
//                            .Where(() => info.Product == products)
//                            .And(() => info.IsInRecovery == isInRecovery)
//                            .List<RAlloc>();
//    return rAllocations;
//}

//public static IList<CAlloc> GetCAllocations(ScbEnums.Products products, bool isInRecovery, ISession session)
//{
//    CAlloc alloc = null;
//    CInfo info = null;
//    GPincode pincode = null;
//    Stakeholders stakeholders = null;
//    var cAllocations = session.QueryOver<CAlloc>(() => alloc)
//                            .Fetch(x => x.CInfo).Eager
//                             .Fetch(x => x.CInfo.GPincode).Eager
//                            .Fetch(x => x.Stakeholder).Eager
//                            .JoinQueryOver(() => alloc.CInfo, () => info, JoinType.InnerJoin)
//                            .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
//                            .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
//                            .Where(() => info.Product == products)
//                            .And(() => info.IsInRecovery == isInRecovery)
//                            .List<CAlloc>();
//    return cAllocations;
//}