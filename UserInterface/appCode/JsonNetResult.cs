using ColloSys.DataLayer.JsonSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AngularUI.appCode
{

    public static class JsonNetResult
    {
        public static JsonSerializerSettings InitSettings(JsonSerializerSettings settings)
        {
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new NHibernateContractResolver();
            //settings.Converters.Add(new NhProxyJsonConverter());
            settings.Converters.Add(new DynamicProxyJsonConverter());
            //settings.Converters.Add(new NhEntityJsonConverter());
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }
    }
}