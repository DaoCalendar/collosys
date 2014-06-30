#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;

#endregion

namespace AngularUI.Stakeholder.view
{
    public static class FilterDataHelper
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        public static IEnumerable<Stakeholders> GetFilteredStakeData(string filterParam)
        {
            switch (filterParam)
            {
                case "All":
                    return StakeQuery.GetAllStakeholders();
                case "Approved":
                    return StakeQuery.GetAllApproved();
                case "Unapproved":
                    return StakeQuery.GetAllUnApproved();
                default:
                    throw new Exception("invalid filter param " + filterParam);
            }
        }
    }
}