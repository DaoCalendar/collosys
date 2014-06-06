using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Billing
{
    public class BMatrix : Entity
    {
        public virtual IList<BMatrixValue> BMatricesValues { get; set; }

        public virtual string Name { get; set; }
        public virtual ScbEnums.Products Products { get; set; }
        public virtual UInt32 Dimension { get; set; }
        public virtual ColloSysEnums.PayoutLRType MatrixPerType { get; set; }
        public virtual string MatrixPerTypeName { get; set; }

        public virtual UInt32 Row1DCount { get; set; }
        public virtual ColloSysEnums.PayoutLRType Row1DType { get; set; }
        public virtual string Row1DTypeName { get; set; }
        public virtual ColloSysEnums.Operators Rows1Operator { get; set; }

        public virtual UInt32 Column2DCount { get; set; }
        public virtual ColloSysEnums.PayoutLRType Column2DType { get; set; }
        public virtual string Column2DTypeName { get; set; }
        public virtual ColloSysEnums.Operators Columns2Operator { get; set; }
    }
}