#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class FileStatus : Entity
    {

        #region properties

        public virtual FileScheduler FileScheduler { get; set; }

        public virtual UInt64 TotalRows { get; set; }

        public virtual UInt64 ValidRows { get; set; }

        public virtual UInt64 UploadedRows { get; set; }

        public virtual UInt64 DuplicateRows { get; set; }

        public virtual UInt64 ErrorRows { get; set; }

        public virtual UInt64 IgnoredRows { get; set; }

        public virtual ColloSysEnums.UploadStatus UploadStatus { get; set; }

        public virtual DateTime EntryDateTime { get; set; }
        #endregion

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //}
        //#endregion
    }
}