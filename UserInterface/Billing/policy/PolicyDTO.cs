using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;
using NHibernate;
using NHibernate.Linq;

namespace AngularUI.Billing.policy
{
    public class PolicyDTO
    {
        public ScbEnums.Products Product;
        public ColloSysEnums.PolicyType PolicyType;
        public ColloSysEnums.PolicyOn PolicyFor;
        public Guid PolicyForId;
        public Guid PolicyId;
        public List<SubpolicyDTO> IsInUseSubpolices = new List<SubpolicyDTO>();
        public List<SubpolicyDTO> NotInUseSubpolices = new List<SubpolicyDTO>();

        public void SetPolicyId(ISession session)
        {
            var billingPolicy = session.Query<BillingPolicy>()
                .SingleOrDefault(x => x.Products == Product &&
                                      x.PolicyType == PolicyType &&
                                      x.PolicyFor == PolicyFor &&
                                      x.PolicyForId == PolicyForId);

            if (billingPolicy != null)
            {
                PolicyId = billingPolicy.Id;
                return;
            }

            billingPolicy = new BillingPolicy
            {
                Category = ScbEnums.Category.Liner,
                Name = "SystemCreated",
                PolicyFor = PolicyFor,
                PolicyForId = PolicyForId,
                PolicyType = PolicyType,
                Products = Product
            };

            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(billingPolicy);
                tx.Commit();
            }
            PolicyId = billingPolicy.Id;
        }
    }
}