#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.SharedDomain;

#endregion

namespace ColloSys.AllocationService.ChurnAllocations
{
    public class AllocationChurn
    {
        public bool Init(IList<Allocations> dataToChange) 
        {
            if (dataToChange == null)
                return false;
            if (dataToChange.Count == 0)
                return true;
            var listOfAllocationData = AssignAllocationDataByStakeholder(dataToChange);
            var revisedlist = CreateChurnListOfAllocations(listOfAllocationData);

            AssignStakeholdersForAllocations(revisedlist);
            return true;
        }

        private static void AssignStakeholdersForAllocations(IEnumerable<AllocationData> revisedlist)
        {
            foreach (var allocationData in revisedlist)
            {
                foreach (var data in allocationData.AllocList)
                {
                    data.Stakeholder = allocationData.Stakeholders;
                }
            }
        }

        private static IEnumerable<AllocationData> CreateChurnListOfAllocations(IReadOnlyList<AllocationData> listOfAllocationData)
        {
            var random = new Random();
            return listOfAllocationData.Select(allocationData => new AllocationData
                {
                    Stakeholders = allocationData.Stakeholders, AllocList = listOfAllocationData[random.Next(1, listOfAllocationData.Count)].AllocList
                }).ToList();
        }

        private static List<AllocationData> AssignAllocationDataByStakeholder<T>(IList<T> dataToChange) where T : Allocations
        {
            var stakeholderlist = dataToChange.Select(x => x.Stakeholder).Distinct().ToList();
            return stakeholderlist.Select(stakeholderse => new AllocationData
                {
                    Stakeholders = stakeholderse, AllocList = dataToChange.Where(x => x.Stakeholder.Id == stakeholderse.Id).ToList<Allocations>()
                }).ToList();
        }
    }
}