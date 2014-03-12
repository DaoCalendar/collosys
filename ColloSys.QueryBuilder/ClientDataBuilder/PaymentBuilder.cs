#region references

using ColloSys.DataLayer.ClientData;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class PaymentBuilder : QueryBuilder<Payment>
    {
        public override QueryOver<Payment, Payment> DefaultQuery()
        {
            return QueryOver.Of<Payment>();
        }
    }
}