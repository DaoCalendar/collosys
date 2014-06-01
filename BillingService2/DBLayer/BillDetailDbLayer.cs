using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.SessionMgr;

namespace BillingService2.DBLayer
{
    internal static class BillDetailDbLayer
    {
        // save list of bill detail
        public static bool SaveBillDetailsBillAmount(BillStatus billStatus, IList<BillDetail> billDetails, BillAmount billAmount, List<DHFL_Liner> dhflLiners, List<DHFL_Info> dhflInfos)
        {
            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(billStatus);

                foreach (var entity in billDetails)
                {
                    session.SaveOrUpdate(entity);
                }

                foreach (var entity in dhflLiners)
                {
                    session.SaveOrUpdate(entity);
                }

                foreach (var entity in dhflInfos)
                {
                    session.SaveOrUpdate(entity);
                }

                session.SaveOrUpdate(billAmount);

                tx.Commit();
            }

            return true;
        }

        public static BillDetail GetOlderBillDetails(Guid id, uint originMonth)
        {
            var session = SessionManager.GetCurrentSession();
            var lastbillmonth = session.QueryOver<BillDetail>()
                .Where(x => x.OriginMonth == originMonth)
                .And(x => x.Stakeholder.Id == id)
                .OrderBy(x => x.BillMonth).Desc
                .List().FirstOrDefault();
            if (lastbillmonth == null) return null;

            var lastbillDetailsList = session.QueryOver<BillDetail>()
                .Where(x => x.OriginMonth == originMonth)
                .And(x => x.BillMonth == lastbillmonth.BillMonth)
                .And(x => x.Stakeholder.Id == id)
                .List();
            if (lastbillDetailsList.Count == 0) return null;

            var newBillDetail = lastbillDetailsList.First();
            session.Evict(newBillDetail);
            newBillDetail.ResetUniqueProperties();

            newBillDetail.Amount = lastbillDetailsList.Sum(x => x.Amount) * (-1);
            newBillDetail.BillAdhoc = null;
            newBillDetail.BillCycle = 0;
            newBillDetail.BillMonth = 0;
            newBillDetail.BillingPolicy = null;
            newBillDetail.BillingSubpolicy = null;
            newBillDetail.OriginMonth = originMonth;
            newBillDetail.PaymentSource = ColloSysEnums.PaymentSource.Adhoc;
            newBillDetail.PolicyType = ColloSysEnums.PolicyType.Payout;
            newBillDetail.CustBillViewModels = null;
            return newBillDetail;
        }
    }
}