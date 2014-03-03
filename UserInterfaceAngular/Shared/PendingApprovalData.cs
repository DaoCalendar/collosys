using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Domain;

namespace ColloSys.UserInterface.Shared
{
    public class PendingApprovalData
    {
        public int stakeholder { get; set; }
        public int working { get; set; }
        public int payment { get; set; }
        public int allocation { get; set; }
        public int allocationpolicy { get; set; }
    }
}