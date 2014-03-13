#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class CWriteoffBuilder : QueryBuilder<CWriteoff>
    {
        public override QueryOver<CWriteoff, CWriteoff> WithRelation()
        {
            return QueryOver.Of<CWriteoff>();
        }
    }
}