#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class StakeAddressMap : EntityMap<StakeAddress>
    {
        public StakeAddressMap()
        {
            Table("STKH_ADDRESS");

            #region Property
            //Property(x => x.Source);
            //Property(x => x.SourceId, map => map.NotNullable(false));
            //Property(x => x.AddressType);
            //Property(x => x.IsOfficial);
            Property(x => x.Line1);
            Property(x => x.Line2, map => map.NotNullable(false));
            Property(x => x.Line3, map => map.NotNullable(false));
            Property(x => x.LandlineNo, map => map.NotNullable(false));
            Property(x => x.Pincode);
            Property(x => x.Country);
            Property(x => x.StateCity, map => map.NotNullable(false));
            #endregion

            #region Approver Component

            Property(p => p.Status);
            Property(p => p.Description, map=> map.NotNullable(false));
            Property(p => p.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);

            #endregion

            #region relations
            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
            //Set(x => x.Stakeholders, colmap => { }, map => map.OneToMany(x => { }));
           // Set(x => x.GPincodes, colmap => { }, map => map.OneToMany(x => { }));

            #endregion
        }
    }
}