using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.PerformanceParameter
{
   public class PerformanceParams:Entity
    {
       public virtual string Param { get; set; }
       public virtual int Weightage { get; set; }
       public virtual ScbEnums.Products Products { get; set; }
    }
}
