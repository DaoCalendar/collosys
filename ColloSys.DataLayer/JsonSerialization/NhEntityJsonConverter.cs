using System;
using System.IO;
using ColloSys.DataLayer.BaseEntity;
using Newtonsoft.Json;

namespace ColloSys.DataLayer.JsonSerialization
{
    public class NhEntityJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var obj = value as Entity;
            if (obj == null)
            {
                throw new InvalidDataException("Can serialize only entity objects");
            }

            obj.MakeEmpty();

            serializer.Serialize(writer, obj);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Entity).IsAssignableFrom(objectType);
        }
    }
}