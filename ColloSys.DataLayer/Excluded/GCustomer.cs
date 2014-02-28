#region References

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GCustomer : Entity,IApproverComponent 
    {
        #region ctors & Relations

        public GCustomer()
        {
            // lists
            CInfos = new List<CInfo>();
            EInfos = new List<EInfo>();
            RInfos = new List<RInfo>();

            // relationship (retrieval using special logic)
            GAddress = new List<GAddress>();

        }

        // list
        public virtual IList<CInfo> CInfos { get; set; }
        public virtual IList<EInfo> EInfos { get; set; }
        public virtual IList<RInfo> RInfos { get; set; }
        public virtual IList<GAddress> GAddress { get; set; }

        // communication address
        public virtual GAddress GCommAddress { get; set; }

        #endregion

        #region Properties

        public virtual UInt64 AddsComId { get; set; }

        [Required]
        public virtual UInt64 XCustId { get; set; }

        [Required]
        public virtual string Name { get; set; }

        public virtual UInt64? MobileNo { get; set; }

        public virtual UInt64? MobileNo2 { get; set; }

        public virtual UInt64? LandlineNo { get; set; }

        public virtual UInt64? LandlineNo2 { get; set; }

        public virtual string EmailId { get; set; }

        public virtual string EmailId2 { get; set; }

        #endregion

        #region Approver Component
        [Required]
        [EnumDataType(typeof(EnumHelper.ApproveStatus))]
        public virtual EnumHelper.ApproveStatus Status { get; set; }

        public virtual string Description { get; set; }

        public virtual string ApprovedBy { get; set; }

        public virtual DateTime? ApprovedOn { get; set; }

        #endregion
    }
}