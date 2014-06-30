using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Allocation
{
   public  class DistributionPercentage:Entity
    {
       public virtual ScbEnums.Products Products { get; set; }
       public virtual decimal TelecallingInhouse { get; set; }
       public virtual decimal TelecallingExternal { get; set; }
       public virtual decimal FieldInhouse { get; set; }
       public virtual decimal FieldExternal { get; set; }
    }
}
