#region References
using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class FileColumn : Entity
    {
        //relationship
        public virtual FileDetail FileDetail { get; set; }

        #region Property
        public virtual UInt32 Position { get; set; }

        public virtual string FileColumnName { get; set; }

        public virtual string Description { get; set; }

        public virtual UInt32 Length { get; set; }

        public virtual ColloSysEnums.FileDataType ColumnDataType { get; set; }

        public virtual string TempColumnName { get; set; }

        public virtual string DateFormat { get; set; }
        #endregion

        #region Date Range Component
        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }
        #endregion

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}