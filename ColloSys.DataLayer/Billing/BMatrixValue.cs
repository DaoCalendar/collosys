using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Billing
{
    public class BMatrixValue : Entity
    {
        public virtual BMatrix BMatrix { get; set; }
        public virtual UInt32 RowNo1D { get; set; }
        public virtual UInt32 ColumnNo2D { get; set; }
        public virtual string Value { get; set; }
        public virtual ColloSysEnums.Operators RowOperator { get; set; }
        public virtual ColloSysEnums.Operators ColumnOperator { get; set; }
    }
}