#region References
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ColloSys.DataLayer.Infra.Shared;
using Newtonsoft.Json;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class RInfo : SharedInfo
    {
        #region Constr and Relation
        public RInfo()
        {
            RWriteoffs = new List<RWriteoff>();
            RAllocs = new List<RAlloc>();
            RLiners = new List<RLiner>();
        }

        //[JsonIgnore]
        public virtual IList<RLiner> RLiners { get; set; }

        public virtual IList<RAlloc> RAllocs { get; set; }
        public virtual IList<RWriteoff> RWriteoffs { get; set; }

        #endregion

        [Required]
        public virtual decimal LoanAmount { get; set; }

        public virtual UInt32 Tenure { get; set; }

        public virtual decimal? Emi { get; set; }

    }
}

//[Required]
//public virtual UInt64 LoanNo { get; set; }

//[Required]
//public virtual string CustomerName { get; set; }