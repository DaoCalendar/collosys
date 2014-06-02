using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.SessionMgr;
using NHibernate.Linq;

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
                    dhflLiners.Where(x => x.Payout == 0).ForEach(x => x.BillDetail = null);
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
            var lastbillmonth = session.Query<BillAmount>()
                .Fetch(x => x.Stakeholder)
                .Where(x => x.OriginMonth == originMonth && x.Stakeholder.Id == id)
                .OrderByDescending(x => x.BillMonth)
                .FirstOrDefault();
            if (lastbillmonth == null) return null;

            var newBillDetail = new BillDetail
            {
                Amount = lastbillmonth.TotalAmount * -1,
                BillAdhoc = null,
                BillCycle = 0,
                BillMonth = 0,
                BillingPolicy = null,
                BillingSubpolicy = null,
                OriginMonth = originMonth,
                PaymentSource = ColloSysEnums.PaymentSource.Reversal,
                PolicyType = ColloSysEnums.PolicyType.Payout,
                CustBillViewModels = null,
                BaseAmount = 0,
                Products = lastbillmonth.Products,
                Stakeholder = lastbillmonth.Stakeholder
            };

            return newBillDetail;
        }
    }
}