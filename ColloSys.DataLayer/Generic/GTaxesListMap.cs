#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class GTaxesListMap : EntityMap<GTaxesList>
    {
        public GTaxesListMap()
        {
            Table("G_TAXES_LIST");

            #region property

            Property(x => x.TaxName, map => map.NotNullable(true));
            Property(x => x.TaxType, map => map.NotNullable(true));
            Property(x => x.ApplicableTo, map => map.NotNullable(true));
            Property(x => x.IndustryZone);
            Property(x => x.ApplyOn, map => map.NotNullable(true));
            Property(x => x.TotSource);
            Property(x => x.Description);

            #endregion

            #region Bags

            Bag(x => x.GTaxDetails, colmap => { }, map => map.OneToMany(x => { }));

            #endregion
        }
    }
}