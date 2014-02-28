using System;

namespace ColloSys.DataLayer.Generic
{
    public class VersionInfo
    {
        public virtual long Version { get; set; }
        public virtual DateTime AppliedOn { get; set; }
    }
}
