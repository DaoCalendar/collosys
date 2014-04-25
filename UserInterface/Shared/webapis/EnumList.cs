using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularUI.Shared.Model
{
    public struct EnumData
    {
        public string Name;
        public IList<string> Value;

        public EnumData(string name, IList<string> values)
        {
            Name = name;
            Value = values;
        }
    }

    public class EnumList
    {
        public IList<EnumData> Enums { get; private set; }

        public EnumList()
        {
            Enums = new List<EnumData>();
        }

        public void AddToList(Type enumtype)
        {
            if (enumtype.IsEnum)
            {
                Enums.Add(new EnumData(enumtype.Name, Enum.GetNames(enumtype).ToList()));
            }
        }

        public void AddToList(string name, IList<string> values)
        {
            Enums.Add(new EnumData(name, values));
        }
    }
}