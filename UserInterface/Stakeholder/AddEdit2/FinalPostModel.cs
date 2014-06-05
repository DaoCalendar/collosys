using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Domain;

namespace ColloSys.UserInterface.Areas.Stakeholder2.Models
{
    public class FinalPostModel
    {
        public FinalPostModel()
        {
            PayWorkModelList=new List<PayWorkModel>();
        }

        
        public StkhHierarchy Hierarchy { get; set; }
        public Stakeholders Stakeholder { get; set; }
        public IList<PayWorkModel> PayWorkModelList { get; set; }
        public StkhRegistration Registration { get; set; }
        public StakeAddress Address { get; set; }
        public string LocationLevel { get; set; }
        public IList<Stakeholders> ReportsToList { get; set; }
        public string EmailId { get; set; }
        public uint Pincode { get; set; }
        public Stakeholders ReportsTo { get; set; }
        public bool IsEditMode { get; set; }
        public PayWorkModel PayWorkModel { get; set; }
        public static void SetFinalPostModel(FinalPostModel model)
        {
        }
    }

    public class PayWorkModel
    {
        public StkhPayment Payment { get; set; }
        public Guid CollectionBillingPolicyId { get; set;}
        public Guid RecoveryBillingPolicyId { get; set;}
        public IList<StkhWorking> WorkList { get; set; }
    }
}