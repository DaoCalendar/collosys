using System.Collections.Generic;

namespace ColloSys.Shared.NgGrid
{
    public class SortConfig
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public IList<string> fields { get; set; }
        public IList<string> directions { get; set; }
        public IList<NgGridColumn> columns { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore InconsistentNaming

        public SortConfig()
        {
            fields = new List<string>();
            directions = new List<string>();
            columns = new List<NgGridColumn>();
        }
    }
}