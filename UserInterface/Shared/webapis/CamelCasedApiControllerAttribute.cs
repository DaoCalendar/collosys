using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using Newtonsoft.Json.Serialization;

public class CamelCasedApiControllerAttribute : Attribute, IControllerConfiguration
{
    public void Initialize(HttpControllerSettings httpControllerSettings, HttpControllerDescriptor httpControllerDescriptor)
    {
        var jsonMediaTypeFormatter = httpControllerSettings.Formatters.OfType<JsonMediaTypeFormatter>().Single();
        httpControllerSettings.Formatters.Remove(jsonMediaTypeFormatter);

        jsonMediaTypeFormatter = new JsonMediaTypeFormatter
        {
            SerializerSettings =
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        httpControllerSettings.Formatters.Add(jsonMediaTypeFormatter);
    }
}