using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;

namespace ColloSys.DataLayer.ClientData
{
    public class FilterCondition : Entity
    {
        public virtual FileDetail FileDetail { get; set; }
        public virtual string RelationType { get; set; }
        public virtual string ColumnName { get; set; }
        public virtual ColloSysEnums.Operators Operator { get; set; }
        public virtual string Value { get; set; }
    }
}
