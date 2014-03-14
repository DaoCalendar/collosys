using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Domain;

namespace ColloSys.UserInterface.Areas.Stakeholder2.Models
{
    public class AddStakeholderModel
    {
        public List<string> LinerPolicy { get { return Enum.GetNames(typeof(LinerPolicies)).ToList(); } }

        public List<string> WriteOfPolicy { get { return Enum.GetNames(typeof(LinerPolicies)).ToList(); } }

        public IList<StkhHierarchy> ListOfStakeHierarchy { get; set; }

        public IDictionary<string, decimal> FixedPay { get; set; }

        public enum LinerPolicies
        {
            Collection_Policy1,
            Collection_Policy2,
            Collection_Policy3
        }

        public enum WriteoffPolicies
        {
            Recovery_Policy1,
            Recovery_Policy2,
            Recovery_Policy3
        }
    }
}