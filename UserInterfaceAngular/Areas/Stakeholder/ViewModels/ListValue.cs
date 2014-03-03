using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserInterfaceAngular.Areas.Stakeholder.ViewModels
{
    public class ListValue
    {
        public string Name { get; set; }
        public Guid Key { get; set; }
    }

    public class ListValueConvert
    {
        public static IList<Guid> ConvertList(IEnumerable<ListValue> list, string keyname)
        {
            var data = list.Where(x => x.Name == keyname).Select(x => x.Key).ToList();
            return data;
        }
    }
}