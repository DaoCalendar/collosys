using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.DataLayer.Generic
{
    public class GKeyValue : Entity
    {
        //#region relationships
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //}

        //#endregion

        #region Properties
        public virtual string Key { get; set; }

        public virtual string Value { get; set; }

        public virtual string ValueType { get; set; }

        public virtual ColloSysEnums.Activities Area { get; set; }
        #endregion
    }
}
