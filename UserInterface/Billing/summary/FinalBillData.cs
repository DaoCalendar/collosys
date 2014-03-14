using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Domain;

namespace ColloSys.UserInterface.Areas.Billing.ViewModels
{
    public class FinalBillData
    {
        public BillAdhoc billAdhoc { get; set; }
        public BillAmount billAmount { get; set; }
    }
}