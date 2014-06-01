using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;

namespace BillingService2.DBLayer
{
    internal static class BillStatusDbLayer
    {
        //// get list of pending billStatus
        //public static void InsertBillStatus()
        //{
        //    var session = SessionManager.GetCurrentSession();

        //    var dhflLiners = session.QueryOver<DHFL_Liner>()
        //                            .List<DHFL_Liner>();

        //    if(dhflLiners.Count<=0)
        //        return;

        //    var billMounth

        //    var billStatus = new BillStatus()
        //    {
        //        BillMonth = 01,
        //        Products = ScbEnums.Products.HL,
        //        Version = 0,
        //        Status = ColloSysEnums.BillingStatus.Pending
        //    };


        //    var billStatus = session.QueryOver<BillStatus>()
        //         .Where(x => x.Status == ColloSysEnums.BillingStatus.Pending).List();

        //    return billStatus;
        //}


        // get list of pending billStatus
        public static IList<BillStatus> GetPendingBillStatus()
        {
            var session = SessionManager.GetCurrentSession();
            var billStatus = session.QueryOver<BillStatus>()
                 .Where(x => x.Status == ColloSysEnums.BillingStatus.Pending)
                 .OrderBy(x => x.BillMonth).Asc.OrderBy(x=>x.OriginMonth).Asc.List();

            return billStatus;
        }

        // save list of bill detail
        public static bool SaveDoneBillStatus(BillStatus billStatus)
        {
            billStatus.Status = ColloSysEnums.BillingStatus.Done;

            var session = SessionManager.GetCurrentSession();

            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(billStatus);
                tx.Commit();
            }

            return true;
        }
    }
}
