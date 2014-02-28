using System;
using NHibernate.Validator.Cfg.Loquacious;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocConditionValidator  : ValidationDef<AllocCondition>
    {
           public AllocConditionValidator()
           {
               Define(x => x.ColumnName).NotNullableAndNotEmpty();
               Define(x => x.AllocSubpolicy).NotNullable();

               ValidateInstance.By((instance, context) => instance.AllocSubpolicy.Id != Guid.Empty)
                               .WithMessage("NhVal : AllocCondition must belong to AllocSubpolicy");
           }
    }
}
