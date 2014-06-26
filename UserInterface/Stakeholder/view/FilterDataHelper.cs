using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

namespace AngularUI.Stakeholder.view
{
    public static class FilterDataHelper
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        public static IEnumerable<Stakeholders> GetFilteredStakeData(string filterParam)
        {
            switch (filterParam)
            {
                case "Approved":
                    return GetApprovedStake();
                case "Unapproved":
                    return GetUnApproved();
                default:
                    throw new Exception("invalid filter param " + filterParam);
            }
        }

        private static IEnumerable<Stakeholders> GetApprovedStake()
        {
            return StakeQuery.GetAllApproved();
        }

        private static IEnumerable<Stakeholders> GetUnApproved()
        {
            return StakeQuery.GetAllUnApproved();
        }
    }
}