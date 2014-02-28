#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class BFormulaMap : EntityMap<BFormula>
    {
        public BFormulaMap()
        {
            Table("B_FORMULA");

            #region property
            Property(x => x.Name);

            Property(x => x.Products);

            Property(x => x.Category);

            #endregion

            #region IApprove
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);

            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);


            #endregion
        }
    }
}