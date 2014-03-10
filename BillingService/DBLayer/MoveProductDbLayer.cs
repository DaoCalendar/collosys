#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using NHibernate.Criterion;

#endregion


namespace BillingService.DBLayer
{
    public class MoveProductDbLayer
    {
        
        public static IList<T> GetpaymentData<T>() where T : Payment
        {
            var _session = SessionManager.GetCurrentSession();
            var data = _session.QueryOver<T>()
                               .Where(x => x.Products == ScbEnums.Products.UNKNOWN)
                               .List();
            return data;
        }

        public static List<TPayment> SetPaymentToProduct<TInfo, TPayment>(IEnumerable<Payment> payments)
            where TInfo : Info
            where TPayment : Payment
        {
            var _session = SessionManager.GetCurrentSession();
            var query = QueryOver.Of<TPayment>()
                                .Where(x => x.Products == ScbEnums.Products.UNKNOWN).Select(x => x.AccountNo);

            var customerinfo = _session.QueryOver<TInfo>().WithSubquery.WhereExists(query).List();

            var paymentList = (from payemnt in payments
                               join info in customerinfo on payemnt.AccountNo equals info.AccountNo
                               select payemnt).ToList();

            paymentList.ForEach(x => x.Products = customerinfo.Single(y => y.AccountNo == x.AccountNo).Product);

            return paymentList.Cast<TPayment>().ToList();
        }

        public static void SaveList(IEnumerable<Entity> payments)
        {
            var _session = SessionManager.GetCurrentSession();
            using (var trans = _session.BeginTransaction())
            {
                foreach (var payment in payments)
                {
                    _session.SaveOrUpdate(payment);
                }
                trans.Commit();
            }
        }
    }
}
