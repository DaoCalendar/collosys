using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.Domain
{
    public class Reports : Entity
    {
        #region Properties
        public virtual string Name { get; set; }

        public virtual string Filter { get; set; }

        public virtual string TableName { get; set; }

        public virtual string Columns { get; set; }

        public virtual string ColumnsFilter { get; set; }

        #endregion

        //#region Relationship None
        //public override void MakeEmpty(bool forceEmpty = false)
        //{
        //    return;
        //}
        //#endregion
    }
}
