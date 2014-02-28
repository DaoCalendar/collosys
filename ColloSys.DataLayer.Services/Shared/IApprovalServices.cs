#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;

#endregion

namespace ColloSys.DataLayer.Services.Shared
{
    public interface IApprovalServices<in T> where T : Entity, IApproverComponent
    {
        void ApproveEntity(T entity);
        void RejectEntity(T entity);
        void SubmitEntityForApproval(T entity, string approvedByUser, bool delete);
    }
}
