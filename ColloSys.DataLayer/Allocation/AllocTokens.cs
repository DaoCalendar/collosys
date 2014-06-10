using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Allocation
{
    public class AllocTokens : Entity
    {
        public virtual string Type { get; set; }
        public virtual string Text { get; set; }
        public virtual string Value { get; set; }
        public virtual string DataType { get; set; }
        public virtual string GroupType { get; set; }
        public virtual int GroupId { get; set; }
        public virtual int Priority { get; set; }
        public virtual AllocSubpolicy AllocSubpolicy { get; set; }

    }
}
