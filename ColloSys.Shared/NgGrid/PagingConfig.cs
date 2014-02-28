using System.Collections.Generic;

namespace ColloSys.Shared.NgGrid
{
    public class PagingConfig
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public ISet<ushort> pageSizes { get; set; }
        public ushort pageSize { get; set; }
        public ulong totalServerItems { get; set; }
        public ushort currentPage { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore InconsistentNaming

        public PagingConfig()
        {
            //pageSizes = new List<ushort> { 250, 500, 1000 };
            pageSizes = new HashSet<ushort>();
            pageSize = 250;
            totalServerItems = 0;
            currentPage = 1;
        }
    }
}