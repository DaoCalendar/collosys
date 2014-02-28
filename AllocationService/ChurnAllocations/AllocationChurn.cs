using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.SharedDomain;

namespace ColloSys.AllocationService.ChurnAllocations
{
    public class AllocationChurn
    {
        public bool Init<T>(IList<T> dataToChange) where T : SharedAlloc
        {
            if (dataToChange == null)
                return false;
            if (dataToChange.Count == 0)
                return true;
            var listOfAllocationData = AssignAllocationDataByStakeholder(dataToChange);
            var revisedlist = CreateChurnListOfAllocations<T>(listOfAllocationData);

            AssignStakeholdersForAllocations<T>(revisedlist);
            return true;
        }

        private static void AssignStakeholdersForAllocations<T>(List<AllocationData> revisedlist) where T : SharedAlloc
        {
            foreach (var allocationData in revisedlist)
            {
                foreach (var data in allocationData.AllocList)
                {
                    data.Stakeholder = allocationData.Stakeholders;
                }
            }
        }

        private static List<AllocationData> CreateChurnListOfAllocations<T>(List<AllocationData> listOfAllocationData) where T : SharedAlloc
        {
            var revisedlist = new List<AllocationData>();
            Random random = new Random();
            foreach (var allocationData in listOfAllocationData)
            {
                var newallocdata = new AllocationData();
                newallocdata.Stakeholders = allocationData.Stakeholders;
                newallocdata.AllocList = listOfAllocationData[random.Next(1, listOfAllocationData.Count)].AllocList;
                revisedlist.Add(newallocdata);
            }
            return revisedlist;
        }

        private static List<AllocationData> AssignAllocationDataByStakeholder<T>(IList<T> dataToChange) where T : SharedAlloc
        {
            var stakeholderlist = dataToChange.Select(x => x.Stakeholder).Distinct().ToList();
            var listOfAllocationData = new List<AllocationData>();
            foreach (var stakeholderse in stakeholderlist)
            {
                var allocationObj = new AllocationData();
                allocationObj.Stakeholders = stakeholderse;
                allocationObj.AllocList = dataToChange.Where(x => x.Stakeholder.Id == stakeholderse.Id).ToList<SharedAlloc>();
                listOfAllocationData.Add(allocationObj);
            }
            return listOfAllocationData;
        }
    }
}