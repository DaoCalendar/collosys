#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.ClientDataBuilder;
using NHibernate.Criterion;

#endregion


namespace BillingService.DBLayer
{
    public class MoveProductDbLayer
    {
        private static readonly PaymentBuilder PaymentBuilder=new PaymentBuilder();
        private static readonly InfoBuilder InfoBuilder=new InfoBuilder();

        public static IList<Payment> GetpaymentData() 
        {
            var data = PaymentBuilder.GetOnExpression(x => x.Products == ScbEnums.Products.UNKNOWN);
            return data;
        }

        public static List<Payment> SetPaymentToProduct(IEnumerable<Payment> payments)
        {
            var query = QueryOver.Of<Payment>()
                                .Where(x => x.Products == ScbEnums.Products.UNKNOWN).Select(x => x.AccountNo);

            var customerinfo = InfoBuilder.ForUnkownProduct(query).ToList();

            var paymentList = (from payemnt in payments
                               join info in customerinfo on payemnt.AccountNo equals info.AccountNo
                               select payemnt).ToList();

            paymentList.ForEach(x => x.Products = customerinfo.Single(y => y.AccountNo == x.AccountNo).Product);

            return paymentList;
        }

        public static void SaveList(IEnumerable<Payment> payments)
        {
            PaymentBuilder.Save(payments.ToList());
        }
    }
}
