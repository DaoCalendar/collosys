#region References
using System;
#endregion

namespace ColloSys.DataLayer.Infra.Domain
{
    public class NlogDb
    {
        public virtual int nlogID { get; set; }

        public virtual DateTime DateTime { get; set; }

        public virtual string Logger { get; set; }

        public virtual string UserName { get; set; }

        public virtual string LogLevel { get; set; }

        public virtual string Message { get; set; }
    }
}
