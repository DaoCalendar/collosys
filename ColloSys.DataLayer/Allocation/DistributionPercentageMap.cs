using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Allocation
{
   public class DistributionPercentageMap:EntityMap<DistributionPercentage>
   {
       public DistributionPercentageMap()
       {
           Property(x=>x.Products);
           Property(x=>x.TelecallingInhouse);
           Property(x=>x.TelecallingExternal);
           Property(x=>x.FieldInhouse);
           Property(x=>x.FieldExternal);
       }
   }
}
