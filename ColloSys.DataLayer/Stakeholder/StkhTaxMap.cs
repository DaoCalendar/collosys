#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class StkhTaxMap : EntityMap<StkhTax>
    {
        public StkhTaxMap()
        {
            Table("STKH_TAXES");

            #region properties

            Property(x => x.TaxType, map => map.NotNullable(true));
            Property(x => x.AmountRange);
            Property(x => x.TaxPercentage, map => map.NotNullable(true));

            #endregion

            #region DateRande Component

            Property(x => x.StartDate, map => map.NotNullable(true));

            Property(x => x.EndDate);
            #endregion

            #region Approve Component
            Property(x => x.ApprovedBy);
            Property(p => p.ApprovedOn, map => map.NotNullable(false));
            Property(x => x.Description);
            Property(x => x.Status, map =>
            {
                map.NotNullable(true);
                map.Length(30);
            });
            #endregion

            #region Relationships - ManyToOne

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));

            #endregion
        }
    }
}