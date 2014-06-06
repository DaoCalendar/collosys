#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;
#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class CUnbilledMap : EntityMap<CUnbilled>
    {
        public CUnbilledMap()
        {
            #region unique key - none

            Property(x => x.AccountNo, map => map.Index("CCMS_IX_UNBILL"));

            Property(x => x.FileDate, map => map.Index("CCMS_IX_UNBILL"));

            Property(x => x.FileRowNo);
            #endregion

            #region Property
            Property(x => x.CustomerName);
            Property(x => x.OriginalTenure);
            Property(x => x.RemainingTenure);
            Property(x => x.InterestPct);
            Property(x => x.LoanAmount);
            Property(x => x.BilledAmount);
            Property(x => x.UnbilledAmount);
            Property(x => x.BilledInterest);

            #endregion

            #region IDateRange
            Property(x => x.StartDate);

            Property(x => x.EndDate);

            #endregion

            ManyToOne(x => x.FileScheduler, map => map.NotNullable(true));
        }
    }
}
