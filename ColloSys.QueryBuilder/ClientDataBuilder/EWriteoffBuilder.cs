#region references

using ColloSys.DataLayer.Domain;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.ClientDataBuilder
{
    public class EWriteoffBuilder : Repository<EWriteoff>
    {
        public override QueryOver<EWriteoff, EWriteoff> ApplyRelations()
        {
            return QueryOver.Of<EWriteoff>();
        }
    }
}