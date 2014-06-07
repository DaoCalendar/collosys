using System;
using System.Collections.Generic;
using System.Linq;

namespace AngularUI.Shared.webapis
{
    public struct EnumData
    {
        public readonly string Name;
        public readonly IList<string> Value;

        public EnumData(string name, IEnumerable<string> values)
        {
            Name = name;
            Value = values.OrderBy(x => x).ToList();
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