using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Generic
{
   public  class PerformanceManagement:Entity
    {
        public virtual IList<PerformanceParams> PerformanceParamses { get; set; }
        public virtual ScbEnums.Products Products { get; set; }

    }
}
