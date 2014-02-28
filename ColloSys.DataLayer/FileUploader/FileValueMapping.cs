using System;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.FileUploader
{
    public class FileValueMapping : Entity
    {
        public virtual string SourceValue { get; set; }
        public virtual string DestinationValue { get; set; }
        public virtual uint Priority { get; set; }
        public virtual FileMapping FileMapping { get; set; }

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion


    }
}
