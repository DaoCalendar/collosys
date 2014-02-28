using System.Collections;

namespace ColloSys.Shared.NgGrid
{
    public class GridQueryResult
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // ReSharper disable UnusedMember.Global
        public IList PageData { get; set; }
        public ulong TotalRowCount { get; set; }
        // ReSharper restore UnusedMember.Global
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global

        public GridQueryResult()
        {
            PageData = new ArrayList();
        }
    }
}