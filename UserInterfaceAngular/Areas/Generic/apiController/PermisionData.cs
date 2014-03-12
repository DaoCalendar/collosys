#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Domain;

#endregion


namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class PermisionData
    {
        public IList<StkhHierarchy> HierarchyData { get; set; }
        public IEnumerable<string> ActivityData { get; set; }
        public IList<GPermission> PermissionData { get; set; }
    }
}