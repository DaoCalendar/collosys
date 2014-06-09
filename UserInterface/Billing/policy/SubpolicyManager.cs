using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Enumerations;

namespace AngularUI.Billing.policy
{
    public class SubpolicyManager
    {
        public IEnumerable<SubpolicyDTO> MoveSubpolicesToDTO(IEnumerable<BillingSubpolicy> subpolicies)
        {
            IList<SubpolicyDTO> dtoList = new List<SubpolicyDTO>();
            foreach (var subpolicy in subpolicies)
            {
                var dto = new SubpolicyDTO();
                dto.Update(subpolicy);
                if (subpolicy.BillingRelations == null || subpolicy.BillingRelations.Count == 0)
                {
                    dtoList.Add(dto);
                    continue;
                }
                var relation = subpolicy.BillingRelations
                    .Where(x => x.Status == ColloSysEnums.ApproveStatus.Approved)
                    .OrderByDescending(x => x.StartDate)
                    .ThenByDescending(x => x.EndDate)
                    .FirstOrDefault();
                if (relation != null)
                {
                    dto.Update(relation);
                    dtoList.Add(dto);
                }

                var relation2 = subpolicy.BillingRelations
                    .Where(x => x.Status == ColloSysEnums.ApproveStatus.Submitted)
                    .OrderByDescending(x => x.StartDate)
                    .ThenByDescending(x => x.EndDate)
                    .FirstOrDefault();
                if (relation2 == null) continue;
                var dto2 = new SubpolicyDTO();
                dto2.Update(subpolicy);
                if (!relation2.EndDate.HasValue || relation2.EndDate > DateTime.Today)
                    dto2.Update(relation2);
                dtoList.Add(dto2);
            }

            return dtoList;
        }

        public void SeperateDTO2List(PolicyDTO policy, IEnumerable<SubpolicyDTO> dtoList)
        {
            foreach (var dto in dtoList)
            {
                dto.PolicyId = policy.PolicyId;
                if (dto.RelationId == Guid.Empty)
                {
                    dto.SubpolicyType = SubpolicyTypeEnum.Draft;
                    policy.NotInUseSubpolices.Add(dto);
                    continue;
                }
                if (dto.ApproveStatus == ColloSysEnums.ApproveStatus.Submitted)
                {
                    dto.SubpolicyType = SubpolicyTypeEnum.Unapproved;
                    policy.IsInUseSubpolices.Add(dto);
                    continue;
                }
                if (dto.EndDate == null || dto.EndDate > DateTime.Today)
                {
                    dto.SubpolicyType = SubpolicyTypeEnum.Active;
                    policy.IsInUseSubpolices.Add(dto);
                    continue;
                }
                dto.SubpolicyType = SubpolicyTypeEnum.Expired;
                policy.NotInUseSubpolices.Add(dto);
            }

            policy.IsInUseSubpolices = policy.IsInUseSubpolices.OrderBy(x => x.Priority).ToList();
        }
    }
}