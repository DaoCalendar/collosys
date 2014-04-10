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
    }

    public class EnumList
    {
        public Dictionary<string, List<string>> enums { get; private set; }

        public EnumList()
        {
            enums = new Dictionary<string, List<string>>();
        }

        public void AddToList(Type enumtype)
        {
            if (enumtype.IsEnum)
            {
                enums.Add(enumtype.Name, Enum.GetNames(enumtype).ToList());
            }
        }

    }
}