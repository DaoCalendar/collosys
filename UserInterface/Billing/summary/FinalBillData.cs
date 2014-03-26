using ColloSys.DataLayer.Domain;

namespace ColloSys.UserInterface.Areas.Billing.ViewModels
{
    public class FinalBillData
    {
        public BillAdhoc billAdhoc { get; set; }
        public BillAmount billAmount { get; set; }
    }
}