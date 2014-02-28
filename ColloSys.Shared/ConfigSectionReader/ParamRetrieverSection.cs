using System.Configuration;

namespace ColloSys.Shared.ConfigSectionReader
{
    public class ColloSysParamsSection : ConfigurationSection
    {
        [ConfigurationProperty("release", IsDefaultCollection = true)]
        public ParamElementCollection ReleaseParams
        {
            get { return (ParamElementCollection)this["release"]; }
            set { this["release"] = value; }
        }

        [ConfigurationProperty("testing", IsDefaultCollection = true)]
        public ParamElementCollection TestingParams
        {
            get { return (ParamElementCollection)this["testing"]; }
            set { this["testing"] = value; }
        }

        [ConfigurationProperty("debug", IsDefaultCollection = true)]
        public ParamElementCollection DebugParams
        {
            get { return (ParamElementCollection)this["debug"]; }
            set { this["debug"] = value; }
        }              
    }
}