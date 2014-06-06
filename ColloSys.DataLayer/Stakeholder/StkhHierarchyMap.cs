using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhHierarchyMap : EntityMap<StkhHierarchy>
    {
        public StkhHierarchyMap()
        {
            Property(x => x.Designation);
            Property(x => x.Hierarchy);
            Property(x => x.LocationLevel);
            Property(x => x.PositionLevel);
            Property(x => x.HasAddress);
            Property(x => x.HasMultipleAddress);
            Property(x => x.HasBankDetails);
            Property(x => x.HasBuckets);
            Property(x => x.IsIndividual);
            Property(x => x.IsUser);
            Property(x => x.HasFixed);
            Property(x => x.HasFixedIndividual);
            Property(x => x.ReportsTo);
            Property(x => x.HasMobileTravel);
            Property(x => x.HasPayment);
            Property(x => x.HasRegistration);
            Property(x => x.HasVarible);
            Property(x => x.HasWorking);
            Property(x => x.HasServiceCharge);
            Property(x => x.ManageReportsTo);
            Property(x => x.IsInAllocation);
            Property(x => x.IsEmployee);
            Property(x => x.IsInField);
            Property(x=>x.ReportingLevel);
            Property(x=>x.WorkingReportsLevel);
            Property(x=>x.WorkingReportsTo);

            Bag(x => x.UsersInRole, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.GPermissions, colmap => { }, map => map.OneToMany(x => { }));
            Bag(x => x.Stakeholders, colmap => { }, map => map.OneToMany(x => { }));
        }
    }
}
