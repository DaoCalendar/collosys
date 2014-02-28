#region references

using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SharedDomain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class AllocMap : EntityMap<Alloc>
    {
        public AllocMap()
        {
            Table("ALLOC");

            #region component

            #region Allocation Component Property

            Property(x => x.IsAllocated);
            Property(x => x.Bucket);
            Property(x => x.AmountDue);
            Property(x => x.ChangeReason, map => map.NotNullable(false));
            Property(x => x.AllocStatus);
            Property(x => x.NoAllocResons);
            #endregion

            #region Date Range Component

            Property(x => x.StartDate);
            Property(x => x.EndDate);

            #endregion

            Property(x => x.WithTelecalling);

            #region Approver Component

            Property(p => p.Status);
            Property(p => p.Description, map => map.NotNullable(false));
            Property(p => p.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(p => p.OrigEntityId);
            Property(p => p.RowStatus);
            #endregion

            #endregion

            #region Relationshiop ManyToOne

            ManyToOne(x => x.AllocPolicy, map => map.NotNullable(false));
            ManyToOne(x => x.AllocSubpolicy, map => map.NotNullable(false));
            ManyToOne(x => x.Stakeholder, map => map.NotNullable(false));
            ManyToOne(x => x.Info, map => map.NotNullable(true));
            #endregion
        }
    }
}