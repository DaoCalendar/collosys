#region References
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class GPermission : Entity
    {
        #region Properties
        public virtual ColloSysEnums.Activities Activity { get; set; }

        public virtual ColloSysEnums.Permissions Permission { get; set; }

        public virtual uint EscalationDays { get; set; }

        public virtual StkhHierarchy Role { get; set; }
        #endregion
        
        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}