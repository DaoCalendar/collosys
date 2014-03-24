#region references

using ColloSys.DataLayer.ClientData;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class PaymentBuilder : Repository<Payment>
    {
        public override QueryOver<Payment, Payment> ApplyRelations()
        {
            return QueryOver.Of<Payment>();
        }
    }
}