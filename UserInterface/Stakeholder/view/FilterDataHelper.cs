#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Stakeholder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using NHibernate.Persister.Entity;

#endregion

namespace AngularUI.Stakeholder.view
{
    public static class FilterDataHelper
    {
        private static readonly StakeQueryBuilder StakeQuery = new StakeQueryBuilder();

        public static PaginationData<Stakeholders> GetFilteredStakeData(PaginationParam param)
        {
            IQueryable<Stakeholders> stakeData;
            switch (param.name)
            {
                case "All":
                    stakeData = StakeQuery.GetAllStakeholders();
                    break;
                case "Approved":
                    stakeData = StakeQuery.GetAllApproved();
                    break;
                case "Unapproved":
                    stakeData = StakeQuery.GetAllUnApproved();
                    break;
                default:
                    throw new Exception("invalid filter param " + param.filter);
            }


            if (!string.IsNullOrWhiteSpace(param.filter))
            {
                stakeData = stakeData.Where(x => x.ExternalId.Contains(param.filter) || x.Name.Contains(param.filter));
            }

            var count = stakeData.Count() ;

            var result = stakeData.Skip((param.page - 1) * (param.size)).Take(param.size).ToList();
            return new PaginationData<Stakeholders>
                {
                    Data = result,
                    Count = count
                };
        }

        public class PaginationData<T>
        {
            public IEnumerable<T> Data { get; set; }
            public int Count { get; set; }
        }
    }
}