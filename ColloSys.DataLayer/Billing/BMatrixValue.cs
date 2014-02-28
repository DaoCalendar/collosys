#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BMatrixValue : Entity
    {
        #region relationship
        public virtual BMatrix BMatrix { get; set; }
        #endregion

        #region property

        public virtual UInt32 RowNo1D { get; set; }

        public virtual UInt32 ColumnNo2D { get; set; }

        public virtual UInt32 RowNo3D { get; set; }

        public virtual UInt32 ColumnNo4D { get; set; }

        public virtual string Value { get; set; }

        public virtual ColloSysEnums.Operators RowOperator { get; set; }

        public virtual ColloSysEnums.Operators ColumnOperator { get; set; }

        #endregion

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}