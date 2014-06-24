using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Generic
{
   public class PerformanceParams:Entity
    {
        public virtual PerformanceManagement PerformanceManagement { get; set; }
        public virtual string Param { get; set; }
        public virtual double Weightage { get; set; }
    }
}
