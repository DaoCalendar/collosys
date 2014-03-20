using System;
using System.Collections.Generic;
using System.Linq;
using FileUploader.Reflection;

namespace FileUploader.ExcelReader
{
    public class ExcelReaderHelper
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        public DateTime Property3 { get; set; }
        public double Property4 { get; set; }
        public float Property5 { get; set; }
        public Int64 Property6 { get; set; }
        public UInt32 Property7 { get; set; }
        public decimal Property8 { get; set; }
        public UInt64 Property9 { get; set; }
        public Int16 Property10 { get; set; }
        public char Property11 { get; set; }

        public static List<MappingInfo> GetMappingInfo()
        {
            return new List<MappingInfo>()
            {
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property1),
                    Position = 1
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property2),
                    Position = 2
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property3),
                    Position = 3
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property4),
                    Position = 4
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property5),
                    Position = 5
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property6),
                    Position = 6
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property7),
                    Position = 7
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property8),
                    Position = 8
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property9),
                    Position = 9
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property10),
                    Position = 10
                },
                new MappingInfo
                {
                    Propname = ReflectionHelper.GetMemberName<ExcelReaderHelper>(x => x.Property11),
                    Position = 11
                }
            };
        }

        public static Type GetTypeOf(string classNam)
        {
            switch (classNam)
            {
                case "ExcelReaderHelper":
                    return typeof(ExcelReaderHelper);
                default:
                    return typeof(ExcelReaderHelper);
            }
        }

        public static string ReadList(IList<ExcelReaderHelper> list, string propname, int index)
        {
            var obj = list.ElementAt(index);
            var proeprty = obj.GetType().GetProperty(propname);
            var value = proeprty.GetValue(obj);
            return value.ToString();
        }

    }
}
