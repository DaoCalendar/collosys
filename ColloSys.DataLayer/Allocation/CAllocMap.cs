#region references

using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class CAllocMap : EntityMap<CAlloc>
    {
        public CAllocMap()
        {
            Table("C_ALLOC");

            #region Allocation Component Property
            Property(x => x.IsAllocated);
            Property(x => x.Bucket);
            Property(x => x.AmountDue);
            Property(x => x.ChangeReason, map => map.NotNullable(false));
            Property(x => x.WithTelecalling);
            Property(x=>x.AllocStatus);
            Property(x=>x.NoAllocResons);
            #endregion

            #region Date Range Component

            Property(x => x.StartDate);
            Property(x => x.EndDate);

            #endregion

            #region Approver Component

            Property(p => p.Status, map =>map.Length(30));
            Property(p => p.Description, map => map.NotNullable(false));
            Property(p => p.ApprovedBy, map => map.NotNullable(false)); 
            Property(p => p.ApprovedOn);
            Property(p => p.OrigEntityId);
            Property(p => p.RowStatus);
            #endregion

            #region Relationshiop ManyToOne

            ManyToOne(x => x.AllocPolicy, map => map.NotNullable(false));
            ManyToOne(x => x.AllocSubpolicy, map => map.NotNullable(false));
            ManyToOne(x => x.Stakeholder);
            ManyToOne(x => x.CLiner);
            ManyToOne(x => x.CWriteoff);
            ManyToOne(x => x.CInfo, map => map.NotNullable(true));
            #endregion


        }
    }
}