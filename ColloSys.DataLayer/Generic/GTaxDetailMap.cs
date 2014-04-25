#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class GTaxDetailMap : EntityMap<GTaxDetail>
    {
        public GTaxDetailMap()
        {
            Table("G_TAX_DETAILS");

            #region Property

            Property(x => x.ApplicableTo, map => map.NotNullable(true));
            Property(x => x.IndustryZone);
            Property(x => x.Country, map => map.NotNullable(true));
            Property(x => x.State, map => map.NotNullable(true));
            Property(x => x.District, map => map.NotNullable(true));
            Property(x => x.Priority, map => map.NotNullable(true));
            Property(x => x.Percentage);
            Property(x => x.TaxId, map => map.NotNullable(true));

            #endregion


            #region DateRange Component

            Property(p => p.StartDate, map => map.NotNullable(true));
            Property(p => p.EndDate);

            #endregion

            #region Relationship

            ManyToOne(x => x.GTaxesList, map => map.NotNullable(true));

            //ManyToOne(x => x.BMatrix);

            #endregion
        }
    }
}