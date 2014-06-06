#region references

using ColloSys.DataLayer.BaseEntity;

#endregion

namespace ColloSys.DataLayer.Stakeholder
{
    public class StkhRegistrationMap : EntityMap<StkhRegistration>
    {
        public StkhRegistrationMap()
        {
            Property(x => x.RegistrationNo);
            Property(x => x.PanNo);
            Property(x => x.TanNo);
            Property(x => x.ServiceTaxno);

            ManyToOne(x => x.Stakeholder, map => map.NotNullable(true));
        }
    }
}