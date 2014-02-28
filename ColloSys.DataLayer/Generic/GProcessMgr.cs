using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Generic
{
    public class GProcessMgr:Entity
    {
        public virtual DateTime FileDate { get; set; }

        [EnumDataType(typeof(ScbEnums.Products))]
        public virtual ScbEnums.Products Product { get; set; }

        [EnumDataType(typeof(ScbEnums.Category))]
        public virtual ScbEnums.Category Category { get; set; }

        [EnumDataType(typeof(ColloSysEnums.Status))]
        public virtual ColloSysEnums.Status Status { get; set; }

        public virtual FileScheduler FileScheduler { get; set; }
    }
}
