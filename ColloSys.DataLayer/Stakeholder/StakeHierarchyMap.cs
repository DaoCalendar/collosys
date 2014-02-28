#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class StkhHierarchyMap : EntityMap<StkhHierarchy>
    {
        public StkhHierarchyMap()
        {
            Table("STKH_HIERARCHY");

            #region Properties

            Property(x => x.Designation, map => map.UniqueKey("UQ_STKH_HIERARCHY"));

            Property(x => x.Hierarchy, map => map.UniqueKey("UQ_STKH_HIERARCHY"));

            Property(x => x.ApplicationName);

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

            Property(x => x.ReportsTo, map => map.UniqueKey("UQ_STKH_HIERARCHY"));

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

            #endregion

            #region Relationship

            Set(x => x.UsersInRole, colmap => { }, map => map.OneToMany(x => { }));

            Set(x => x.GPermissions, colmap => { }, map => map.OneToMany(x => { }));

            Set(x => x.Stakeholders, colmap => { }, map => map.OneToMany(x => { }));

            #endregion
        }
    }
}
