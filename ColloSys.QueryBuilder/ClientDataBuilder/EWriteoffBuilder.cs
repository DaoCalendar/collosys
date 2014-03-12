#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class EWriteoffBuilder : QueryBuilder<EWriteoff>
    {
        public override QueryOver<EWriteoff, EWriteoff> DefaultQuery()
        {
            return QueryOver.Of<EWriteoff>();
        }
    }
}