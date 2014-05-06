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
using ColloSys.DataLayer.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using NHibernate;
using NHibernate.SqlCommand;

#endregion


namespace BillingService.CustBillView
{
    public class ProcessCustBillView
    {
    //    public static List<CustBillViewModel> GetBillingServiceData(ScbEnums.Products products)
    //    {
    //        var billingViewModel = new List<CustBillViewModel>();

    //        var session = SessionManager.GetCurrentSession();

    //        var allocations = GetAllocations(products, session);
    //        var payments = GetPayment<Payment>(session);
    //        billingViewModel.AddRange(allocations
    //                                      .Select(allocation =>
    //                                              CreateCustBillViewModel(allocation, allocation.Info, payments))
    //                                      .ToList());


    //        return billingViewModel;
    //    }

    //    public static List<CustBillViewModel> GetBillingServiceData(ScbEnums.Products products, bool isInRecovery)
    //    {
    //        var billingViewModel = new List<CustBillViewModel>();

    //        var session = SessionManager.GetCurrentSession();

    //        var systemOnProduct = Util.GetSystemOnProduct(products);

    //        switch (systemOnProduct)
    //        {
    //            case ScbEnums.ScbSystems.CCMS:
    //                var cAllocations = GetAllocations(products, isInRecovery, session);
    //                var cPayments = GetPayment<Payment>(session);
    //                billingViewModel.AddRange(cAllocations
    //                                              .Select(allocation =>
    //                                                      CreateCustBillViewModel(allocation, allocation.Info, cPayments))
    //                                              .ToList());
    //                break;

    //            case ScbEnums.ScbSystems.EBBS:
    //                var eAllocations = GetAllocations(products, isInRecovery, session);
    //                var ePayments = GetPayment<Payment>(session);
    //                billingViewModel.AddRange(eAllocations
    //                                              .Select(allocation =>
    //                                                      CreateCustBillViewModel(allocation, allocation.Info, ePayments))
    //                                              .ToList());
    //                break;

    //            case ScbEnums.ScbSystems.RLS:
    //                var rAllocations = GetAllocations(products, isInRecovery, session);
    //                var rPayments = GetPayment<Payment>(session);
    //                billingViewModel.AddRange(rAllocations
    //                                              .Select(allocation =>
    //                                                      CreateCustBillViewModel(allocation, allocation.Info, rPayments))
    //                                              .ToList());
    //                break;
    //            default:
    //                throw new ArgumentOutOfRangeException();
    //        }

    //        return billingViewModel;
    //    }

        public static List<CustBillViewModel> GetBillingServiceData(ScbEnums.Products products, uint billMonth)
        {
            var billingViewModel = new List<CustBillViewModel>();

            var session = SessionManager.GetCurrentSession();

            

            var startDate = DateTime.ParseExact(string.Format("{0}01", billMonth), "yyyyMMdd",
                                                CultureInfo.InvariantCulture); ;
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var allocations = GetAllocations(products, startDate, endDate, session);
            var payments = GetPayment<Payment>(session);
            billingViewModel.AddRange(allocations
                                          .Select(allocation =>
                                                  CreateCustBillViewModel(allocation, allocation.Info, payments))
                                          .ToList());
            
            return billingViewModel;
        }

        public static IList<Payment> GetPayment<T>(ISession session) where T : Payment
        {
            var cPayments = session.QueryOver<T>().List<Payment>();
            return cPayments;
        }

        public static CustBillViewModel CreateCustBillViewModel(Allocations allocation, CustomerInfo info, IList<Payment> payments)
        {
            var custBilViewModel = new CustBillViewModel();

            custBilViewModel.AccountNo = info.AccountNo;
            custBilViewModel.GlobalCustId = info.GlobalCustId;
            custBilViewModel.Flag = info.Flag;
            custBilViewModel.Product = info.Product;
            custBilViewModel.Cycle = info.Cycle;
            custBilViewModel.Bucket = (uint)allocation.Bucket;
            custBilViewModel.IsInRecovery = info.IsInRecovery;
            custBilViewModel.IsXHoldAccount = info.IsXHoldAccount;
            custBilViewModel.CityCategory = info.GPincode.CityCategory;
            custBilViewModel.GPincode = info.GPincode;
            custBilViewModel.AllocationStartDate = allocation.StartDate;
            custBilViewModel.AllocationEndDate = allocation.EndDate;
            custBilViewModel.TotalDueOnAllocation = allocation.AmountDue;
            custBilViewModel.MobWriteoff = Util.GetMobWriteoff(info.ChargeofDate);
            custBilViewModel.Vintage = Util.GetVintage(custBilViewModel.MobWriteoff, custBilViewModel.Product);

            var test1 = payments.Where(x => x.AccountNo == custBilViewModel.AccountNo);
            var test2 = payments.Where(x => x.AccountNo == custBilViewModel.AccountNo
                                            && (x.IsDebit && !x.IsExcluded));


            var tets = payments.Where(x => (x.AccountNo == custBilViewModel.AccountNo)
                                           && (x.TransDate.Date >= custBilViewModel.AllocationStartDate.Date)
                                           && (custBilViewModel.AllocationEndDate != null && x.TransDate.Date <= custBilViewModel.AllocationEndDate.Value.Date)
                                           && (x.IsDebit)
                                           && (!x.IsExcluded));

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

        #region get allocations

        public static IList<Allocations> GetAllocations(ScbEnums.Products products, bool isInRecovery, ISession session)
        {
            Allocations alloc = null;
            CustomerInfo info = null;
            GPincode pincode = null;
            Stakeholders stakeholders = null;
            var eAllocations = session.QueryOver<Allocations>(() => alloc)
                                    .Fetch(x => x.Info).Eager
                                    .Fetch(x => x.Info.GPincode).Eager
                                    .Fetch(x => x.Stakeholder).Eager
                                    .JoinQueryOver(() => alloc.Info, () => info, JoinType.InnerJoin)
                                    .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
                                    .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
                                    .Where(() => info.Product == products)
                                    .And(() => info.IsInRecovery == isInRecovery)
                                    .List<Allocations>();
            return eAllocations;
        }

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

        #region by Product

        public static IList<Allocations> GetAllocations(ScbEnums.Products products, ISession session)
        {
            Allocations alloc = null;
            CustomerInfo info = null;
            GPincode pincode = null;
            Stakeholders stakeholders = null;
            var eAllocations = session.QueryOver<Allocations>(() => alloc)
                                    .Fetch(x => x.Info).Eager
                                    .Fetch(x => x.Info.GPincode).Eager
                                    .Fetch(x => x.Stakeholder).Eager
                                    .JoinQueryOver(() => alloc.Info, () => info, JoinType.InnerJoin)
                                    .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
                                    .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
                                    .Where(() => info.Product == products)
                                    .List<Allocations>();
            return eAllocations;
        }

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

        #endregion

        #region start date end date data
        public static IList<Allocations> GetAllocations(ScbEnums.Products products, DateTime startDate, DateTime endDate, ISession session)
        {
            Allocations alloc = null;
            CustomerInfo info = null;
            GPincode pincode = null;
            Stakeholders stakeholders = null;
            var cAllocations = session.QueryOver<Allocations>(() => alloc)
                                    .Fetch(x => x.Info).Eager
                                     .Fetch(x => x.Info.GPincode).Eager
                                    .Fetch(x => x.Stakeholder).Eager
                                    .JoinQueryOver(() => alloc.Info, () => info, JoinType.InnerJoin)
                                    .JoinQueryOver(() => info.GPincode, () => pincode, JoinType.InnerJoin)
                                    .JoinQueryOver(() => alloc.Stakeholder, () => stakeholders, JoinType.InnerJoin)
                                    .Where(() => info.Product == products)
                                    .And(() => info.AllocStartDate >= startDate && info.AllocEndDate <= endDate)
                                    .List<Allocations>();
            return cAllocations;
        }

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

        #endregion


        #endregion
    }
}
