using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using ColloSys.DataLayer.JsonSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UserInterfaceAngular.Shared
{

    public class JsonNetResult : JsonResult
    {
        private JsonSerializerSettings Settings { get; set; }

        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings();
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            InitSettings(Settings);
        }

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

        public override void ExecuteResult(ControllerContext context)
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("JSON GET is not allowed");
            }

            // set json response
            var response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            // set utf-8 encoding
            if (ContentEncoding == null)
            {
                response.ContentEncoding = Encoding.UTF8;
            }

            // data is must for response
            if (Data == null)
            {
                return;
            }

            // create serializer and return the response
            var scriptSerializer = JsonSerializer.Create(Settings);

            using (var sw = new StringWriter())
            {
                scriptSerializer.Serialize(sw, Data);
                response.Write(sw.ToString());
            }
        }
    }
}



//settings.Converters.Add(new IndiaDateTimeConvertor());

//public class IndiaDateTimeConvertor : DateTimeConverterBase
//{
//    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//    {
//        if ((reader.Value != null) && (!string.IsNullOrWhiteSpace(reader.Value.ToString())))
//        {
//            return DateTime.Parse(reader.Value.ToString());
//        }
//        return null;
//    }

//    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//    {
//        writer.WriteValue(((DateTime)value).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'-00:00'"));
//        //writer.WriteValue(((DateTime)value).ToString("u"));
//    }
//}

