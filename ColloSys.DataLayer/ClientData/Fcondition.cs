using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.ClientData
{
   public class Fcondition:Entity
    {
        public virtual string RelationType { get; set; }
        public virtual string ColumnName { get; set; }
        public virtual ColloSysEnums.Operators Operator { get; set; }
        public virtual string Value { get; set; }
       public virtual FilterCondition FilterCondition { get; set; }
    }
}
