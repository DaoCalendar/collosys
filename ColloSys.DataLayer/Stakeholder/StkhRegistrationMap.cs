#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class StkhRegistrationMap : EntityMap<StkhRegistration>
    {
        public StkhRegistrationMap()
        {
            Table("STKH_REGISTRATION");

            #region properties

            Property(x => x.HasCollector);
            Property(x => x.RegistrationNo);
            Property(x => x.PanNo);
            Property(x => x.TanNo);
            Property(x => x.ServiceTaxno);

            #endregion

            #region IApprove
            Property(x => x.ApprovedBy, map => map.NotNullable(false));
            Property(p => p.ApprovedOn);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.Status);
            Property(x => x.OrigEntityId);
            Property(x => x.RowStatus);
				
            #endregion

            #region Relationships - ManyToOne

            ManyToOne(x => x.Stakeholder, map =>
                {
                    map.NotNullable(true);
                    map.UniqueKey("UQ_STAKEHOLDER_REGISTRATION");
                });

            #endregion
        }
    }
}