using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StakeholdersMap : EntityMap<Stakeholders>
    {
        public StakeholdersMap()
        {
            Table("Stakeholder");

            Property(x => x.ExternalId, map => map.NotNullable(false));
            Property(x => x.Name);
            Property(x => x.MobileNo);
            Property(x => x.EmailId);
            Property(x => x.ReportingManager);
            Property(x => x.JoiningDate);
            Property(x => x.LeavingDate);
            Property(x => x.ApprovalStatus);

            ManyToOne(x => x.Hierarchy, map => map.NotNullable(false));

            Bag(x => x.BillAdhocs, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.BillAmounts, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.BillDetails, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.Allocs, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.StkhPayments, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.StkhRegistrations, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.StkhWorkings, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.AllocSubpolicies, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.StkhAddress, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.ActivateHoldingPolicies, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.ReportsAllocs, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}
