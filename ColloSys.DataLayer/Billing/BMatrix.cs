#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using Iesi.Collections.Generic;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BMatrix : Entity, IApproverComponent, IPolicyStatusComponent
    {
        #region Relation

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    if (!NHibernateUtil.IsInitialized(BMatricesValues) || forceEmpty) BMatricesValues = null;
        //}

        public virtual ISet<BMatrixValue> BMatricesValues { get; set; }
        #endregion

        #region property

        public virtual string Name { get; set; }

        public virtual UInt32 Dimension { get; set; }

        public virtual UInt32 Row1DCount { get; set; }

        public virtual ColloSysEnums.PayoutLRType Row1DType { get; set; }

        public virtual string Row1DTypeName { get; set; }

        public virtual UInt32? Column2DCount { get; set; }

        public virtual ColloSysEnums.PayoutLRType Column2DType { get; set; }

        public virtual string Column2DTypeName { get; set; }

        public virtual UInt32? Row3DCount { get; set; }

        public virtual ColloSysEnums.PayoutLRType Row3DType { get; set; }

        public virtual string Row3DTypeName { get; set; }

        public virtual UInt32? Column4DCount { get; set; }

        public virtual ColloSysEnums.PayoutLRType Column4DType { get; set; }

        public virtual string Column4DTypeName { get; set; }

        public virtual bool IsPercentile { get; set; }

        public virtual ColloSysEnums.Operators RowsOperator { get; set; }

        public virtual ColloSysEnums.Operators ColumnsOperator { get; set; }

        public virtual ColloSysEnums.PayoutLRType MatrixPerType { get; set; }

        public virtual string MatrixPerTypeName { get; set; }


        #endregion

        #region IStatusComponent
        public virtual bool IsActive { get; set; }
        public virtual bool IsInUse { get; set; }
        #endregion

        #region IProduct
        public virtual ScbEnums.Products Products { get; set; }

        public virtual ScbEnums.Category Category { get; set; }
        #endregion

        #region IApprove
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion
    }
}