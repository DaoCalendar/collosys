using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Enumerations;
using NHibernate;
using NHibernate.Linq;

namespace AngularUI.Allocation.policy
{
    public class PolicyDTO
    {
        public ScbEnums.Products Product;
        public Guid PolicyId;
        public List<SubpolicyDTO> IsInUseSubpolices = new List<SubpolicyDTO>();
        public List<SubpolicyDTO> NotInUseSubpolices = new List<SubpolicyDTO>();

        public void SetPolicyId(ISession session)
        {
            var allocPolicy = session.Query<AllocPolicy>()
                .SingleOrDefault(x => x.Products == Product);

            if (allocPolicy != null)
            {
                PolicyId = allocPolicy.Id;
                return;
            }

            allocPolicy = new AllocPolicy
            {
                Name = "SystemCreated",
                Products = Product
            };

            using (var tx = session.BeginTransaction())
            {
                session.SaveOrUpdate(allocPolicy);
                tx.Commit();
            }
            PolicyId = allocPolicy.Id;
        }
    }

}