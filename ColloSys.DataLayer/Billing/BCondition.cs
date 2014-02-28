#region references
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using System;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class BCondition : Entity
    {
        #region Properties

        public virtual ColloSysEnums.PayoutLRType Ltype { get; set; }

        public virtual string LtypeName { get; set; }

        public virtual ColloSysEnums.Lsqlfunction Lsqlfunction { get; set; }

        public virtual ColloSysEnums.Operators Operator { get; set; }

        public virtual ColloSysEnums.PayoutLRType Rtype { get; set; }

        public virtual string RtypeName { get; set; }

        public virtual string Rvalue { get; set; }

        public virtual string RelationType { get; set; }

        public virtual uint Priority { get; set; }

        public virtual ColloSysEnums.ConditionType ConditionType { get; set; }

        public virtual BillingSubpolicy BillingSubpolicy { get; set; }

        #endregion

        //#region relationships - none

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}

        //#endregion

    }
}