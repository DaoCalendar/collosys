#region references

using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GReportBuilder : Repository<GReports>
    {
        public override QueryOver<GReports, GReports> ApplyRelations()
        {
            return QueryOver.Of<GReports>();
        }
    }
}