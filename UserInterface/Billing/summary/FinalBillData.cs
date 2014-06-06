using ColloSys.DataLayer.Billing;

namespace ColloSys.UserInterface.Areas.Billing.ViewModels
{
    public class FinalBillData
    {
        public BillAdhoc billAdhoc { get; set; }
        public BillSummary billAmount { get; set; }
    }
}