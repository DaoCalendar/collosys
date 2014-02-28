#region References

using System.Collections.Generic;
using ColloSys.DataLayer.Infra.Shared;

#endregion

namespace ColloSys.DataLayer.Domain
{
    public class EInfo : SharedInfo 
    {
        #region Constructor & Relations
        public EInfo()
        {
            EAllocs = new List<EAlloc>();
            ELiners = new List<ELiner>();
            EWriteoffs = new List<EWriteoff>();
        }

        public virtual IList<EAlloc> EAllocs { get; set; }
        public virtual IList<ELiner> ELiners { get; set; }
        public virtual IList<EWriteoff> EWriteoffs { get; set; }

        //class
       
        #endregion

        #region Properties
        public virtual decimal? OdLimit { get; set; }

        #endregion
    }
}