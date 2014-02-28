using System.Configuration;

namespace ColloSys.Shared.ConfigSectionReader
{
    [ConfigurationCollection(typeof(ParamElement))]
    public class ParamElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ParamElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParamElement)element).Name;
        }
    }
}