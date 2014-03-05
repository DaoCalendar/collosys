﻿#region References
using System;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class StkhWorking : Entity, IApproverComponent //, IProductComponent
    {
        public virtual Stakeholders Stakeholder { get; set; }
        public virtual GPincode GPincode { get; set; }
        public virtual StkhPayment StkhPayment { get; set; }

        #region Properties

        public virtual Guid ReportsTo { get; set; }

        public virtual uint BucketStart { get; set; }

        public virtual uint? BucketEnd { get; set; }

        public virtual string Country { get; set; }

        public virtual string Region { get; set; }

        public virtual string State { get; set; }

        public virtual string Cluster { get; set; }

        public virtual string District { get; set; }

        public virtual string City { get; set; }

        public virtual string Area { get; set; }

        public virtual ScbEnums.Products Products { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual string LocationLevel { get; set; }
        #endregion

        #region Approver
        public virtual ColloSysEnums.ApproveStatus Status { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApprovedBy { get; set; }
        public virtual DateTime? ApprovedOn { get; set; }
        public virtual Guid OrigEntityId { get; set; }
        public virtual RowStatus RowStatus { get; set; }

        #endregion

        //#region Relationship None

        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}