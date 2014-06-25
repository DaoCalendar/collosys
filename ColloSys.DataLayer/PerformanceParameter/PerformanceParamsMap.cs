using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.PerformanceParameter
{
   public  class PerformanceParamsMap:EntityMap<PerformanceParams>
   {
       public PerformanceParamsMap()
       {
           Property(x => x.Param);
           Property(x => x.Weightage);
           Property(x => x.Products);
       }
   }
}
