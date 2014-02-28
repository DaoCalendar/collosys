#region References
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.Infra.Shared;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class CInfo : SharedInfo
    {
        #region Constructor & Relation
        public CInfo()
        {
            CLiners = new List<CLiner>();
            CWriteoffs = new List<CWriteoff>();
            CAllocs = new List<CAlloc>();
        }

        //Collection
        public virtual IList<CLiner> CLiners { get; set; }
        public virtual IList<CWriteoff> CWriteoffs { get; set; }
        public virtual IList<CAlloc> CAllocs { get; set; }
        public virtual GCustomer GCustomer { get; set; }
        #endregion

        #region Properties
        [Editable(false)]
        public virtual UInt64? CustId { get; set; }

        [Editable(false)]
        public virtual UInt64? CreditLimit { get; set; }
        #endregion
     }
}

