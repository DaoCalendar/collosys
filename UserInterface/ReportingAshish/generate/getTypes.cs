using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ColloSys.UserInterface.Areas.Reporting.ViewModels
{
    public class getTypes
    {
        public static string getJSType(PropertyInfo propertyinfo)
        {
           switch (GetCoreType(propertyinfo.PropertyType).Name.ToUpper())
                {
                    case "INT16":
                        return "number";
                    case "UINT16":
                        return "number";
                    case "SHORT":
                        return "number";
                    case "USHORT":
                        return "number";
                    case "INT32":
                        return "number";
                    case "UINT32":
                        return "number";
                    case "INT":
                        return "number";
                    case "UINT":
                        return "number";
                    case "INT64":
                        return "longnumber";
                    case "UINT64":
                        return "longnumber";
                    case "LONG":
                        return "longnumber";
                    case "ULONG":
                        return "longnumber";
                    case "DECIMAL":
                        return "decimal";
                    case "DOUBLE":
                        return "float";
                    case "BIGINT":
                        return "longnumber";
                    case "STRING":
                        return "text";
                    case "DATETIME":
                        return "datepicker";
                    case "BOOLEAN":
                        return "flag";
                   case "ENUM":
                        return "enum";
                }
            //}
            return "text";
        }

        public static string getpropertyType(string JSType)
        {
            switch (JSType.ToUpper())
            {
                case "NUMBER":
                    return "uint32";
                case "LONGNUMBER":
                    return "uint64";
                case "DECIMAL":
                    return "decimal";
                case "FLOAT":
                    return "double";
                case "TEXT":
                    return "String";
                case "DATEPICKER":
                    return "DateTime";
                case "FLAG":
                    return "Boolean";
            }
            return "String";
        }


        public static Type GetCoreType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            { return Nullable.GetUnderlyingType(type); }
            else if (type.UnderlyingSystemType.BaseType == typeof(Enum))
            {
                return type.UnderlyingSystemType.BaseType;
            }
            else
                return type;
        }
    }
}