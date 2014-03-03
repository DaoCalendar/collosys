using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Domain;

namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class PermisionData
    {
        public IList<StkhHierarchy> HierarchyData { get; set; }
        public IEnumerable<string> ActivityData { get; set; }
        public IList<GPermission> PermissionData { get; set; }
    }
}