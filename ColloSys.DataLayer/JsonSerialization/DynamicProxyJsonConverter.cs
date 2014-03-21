#region references

using System;
using System.Collections;
using NHibernate.Collection;
using Newtonsoft.Json;

#endregion

namespace ColloSys.DataLayer.JsonSerialization
{
    public class DynamicProxyJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            
            if (value is AbstractPersistentCollection &&
                ((AbstractPersistentCollection) value).WasInitialized)
            {
                writer.WriteStartArray();
                foreach (var item in ((IEnumerable) value))
                {
                    serializer.Serialize(writer, item);
                }
                writer.WriteEndArray();
                return;
            }

            writer.WriteNull();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(AbstractPersistentCollection).IsAssignableFrom(objectType);
        }
    }
}




//return objectType.Name.EndsWith("Proxy") &&
//       (objectType.GetInterfaces().Any(
//           iface => iface == typeof(INHibernateProxy) || iface == typeof(IProxy)));

//public class NhProxyJsonConverter : JsonConverter
//{
//    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//    {
//        writer.WriteNull();
//    }

//    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//    {
//        throw new NotImplementedException();
//    }

//    public override bool CanConvert(Type objectType)
//    {
//        return //typeof(AbstractPersistentCollection).IsAssignableFrom(objectType) || 
//            typeof(INHibernateProxy).IsAssignableFrom(objectType);
//    }
//}

//public class NhContractResolver : DefaultContractResolver
//{
//    protected override JsonContract CreateContract(Type objectType)
//    {
//        return base.CreateContract(
//            typeof(INHibernateProxy).IsAssignableFrom(objectType)
//                ? objectType.BaseType
//                : objectType);
//    }
//}

//public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//{
//    //if (value is AbstractPersistentCollection && ((AbstractPersistentCollection)value).WasInitialized)
//    //{
//    //    writer.WriteStartArray();
//    //    foreach (var item in ((IEnumerable)value))
//    //    {
//    //        serializer.Serialize(writer, item);
//    //    }
//    //    return;
//    //}
//    writer.WriteNull();
//}

//public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//{
//    throw new NotImplementedException();
//}

//public override bool CanConvert(Type objectType)
//{
//    return typeof(INHibernateProxy).IsAssignableFrom(objectType);
//    //if (typeof(AbstractPersistentCollection).IsAssignableFrom(objectType))
//    //{
//    //    return true;
//    //}

//    //return objectType.Name.EndsWith("Proxy") &&
//    //       (objectType.GetInterfaces().Any(iface => iface == typeof(INHibernateProxy) || iface == typeof(IProxy)));

//}


//settings.ContractResolver = new NHibernateContractResolver();
//settings.ContractResolver = new IgnoreSerializableJsonContractResolver();
//settings.ContractResolver = new DefaultContractResolver(true) { IgnoreSerializableInterface = true };
//settings.Converters.Add(new NHibernateProxyConverter());




//public class NHibernateProxyConverter : JsonConverter
//{
//    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//    {
//        return;
//    }

//    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//    {
//        return string.Empty;
//    }

//    public override bool CanConvert(Type objectType)
//    {
//        return typeof(INHibernateProxy).IsAssignableFrom(objectType);
//    }
//}

//    public class NHibernateProxyJsonSerializer : JsonSerializer
//    {
//        private static readonly MemberInfo[] NHibernateProxyInterfaceMembers = typeof(INHibernateProxy).GetMembers();

//        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
//        {
//            List<MemberInfo> members = base.GetSerializableMembers(objectType);

//            members.RemoveAll(delegate(MemberInfo memberInfo)
//            {
//                // We only want to serialize those members which are declared on the proxy, and are not part of the
//                // INHibernateProxy interface or mixed-in by DynamicProxy. 
//                return
//                (IsMemberPartOfNHibernateProxyInterface(memberInfo)) ||
//                (IsMemberDynamicProxyMixin(memberInfo)) ||
//                (IsMemberMarkedWithIgnoreAttribute(memberInfo, objectType)) ||
//                (IsMemberInheritedFromProxySuperclass(memberInfo, objectType));
//            });

//            return members;
//        }

//        private static bool IsMemberPartOfNHibernateProxyInterface(MemberInfo memberInfo)
//        {
//            return Array.Exists(NHibernateProxyInterfaceMembers, delegate(MemberInfo mi) { return memberInfo.Name == mi.Name; });
//        }

//        private static bool IsMemberDynamicProxyMixin(MemberInfo memberInfo)
//        {
//            return memberInfo.Name == "__interceptors";
//        }

//        private static bool IsMemberMarkedWithIgnoreAttribute(MemberInfo memberInfo, Type objectType)
//{
//return objectType.BaseType.GetMember(memberInfo.Name)0.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Length > 0;
//}

//        private static bool IsMemberInheritedFromProxySuperclass(MemberInfo memberInfo, Type objectType)
//        {
//            return memberInfo.DeclaringType != objectType;
//        }
//    }


//public class NHibernateContractResolver : DefaultContractResolver
//{
//    protected override JsonContract CreateContract(Type objectType)
//    {
//        //NHibernate.Proxy.DynamicProxy.IProxy
//        if (typeof(INHibernateProxy).IsAssignableFrom(objectType))
//        {
//            //return base.CreateContract(IgnoreSerializableJsonContractResolver(objectType.BaseType));
//            //return CreateISerializableContract(objectType.BaseType);
//            //return null;
//            return CreateObjectContract(objectType.BaseType);
//        }

//        return base.CreateContract(objectType);
//    }
//}

//public class IgnoreSerializableJsonContractResolver : DefaultContractResolver
//{
//    protected override JsonContract CreateContract(Type objectType)
//    {
//        /* Behavior in base we're overriding:
//        //if (typeof(ISerializable).IsAssignableFrom(objectType))
//        //    return CreateISerializableContract(objectType);
//        */

//        //if (objectType.IsAutoClass
//        //      && objectType.Namespace == null
//        //      && typeof(ISerializable).IsAssignableFrom(objectType))
//        //{

//        //    return base.IgnoreSerializableInterface = objectType);
//        //}
//        base.IgnoreSerializableInterface = true;

//        return base.CreateContract(objectType);
//    }
//}
