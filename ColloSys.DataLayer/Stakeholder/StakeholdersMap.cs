#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;
using NHibernate.Mapping.ByCode;
#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class StakeholdersMap : EntityMap<Stakeholders>
    {
        public StakeholdersMap()
        {
            Table("STAKEHOLDER");


            #region Property

            //Property(x => x.Hierarchy);
            //Property(x => x.Designation);
            Property(x => x.ExternalId, map => map.NotNullable(false));
            Property(x => x.Name);
            Property(x => x.MobileNo);
            Property(x => x.EmailId);
            Property(x => x.ReportingManager);
            Property(x => x.Gender);
            //Property(x => x.Password);
            Property(x => x.JoiningDate);
            Property(x => x.LeavingDate);
            Property(x => x.BirthDate);
            //Property(x => x.IsAddressChange);
            //Property(x => x.IsPaymentChange);
            //Property(x => x.IsWorkingChange);
           // Property(x => x.LocationLevel);
            //Property(x => x.HierarchyId);

            #endregion

            #region Approve Component

            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x=>x.OrigEntityId);
            Property(x=>x.RowStatus);

            #endregion

            #region Mapping

            ManyToOne(x => x.Hierarchy, map => map.NotNullable(false));

            //ManyToOne(x => x.GCommAddress, map => map.NotNullable(false));//comment this mahendra

            #endregion

            #region Bags
            Set(x => x.BillAdhocs, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.BillAmounts, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.BillDetails, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.CAllocs, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.EAllocs, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.RAllocs, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.StkhPayments, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.StkhRegistrations, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.StkhWorkings, colmap => { }, map => map.OneToMany(x => { }));
            Set(x => x.AllocSubpolicies, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.GAddress, colmap => { }, map => map.OneToMany(x => { }));
            #endregion

        }
    }
}
