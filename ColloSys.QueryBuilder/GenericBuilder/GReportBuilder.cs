#region references

using ColloSys.DataLayer.Generic;
using ColloSys.QueryBuilder.BaseTypes;
using NHibernate.Criterion;

#endregion


namespace ColloSys.QueryBuilder.GenericBuilder
{
    public class GReportBuilder : QueryBuilder<GReports>
    {
        public override QueryOver<GReports, GReports> DefaultQuery()
        {
            return QueryOver.Of<GReports>();
        }
    }
}