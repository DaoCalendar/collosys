#region references

using System.Collections;
using System.Collections.Generic;

#endregion

namespace ColloSys.Shared.NgGrid
{
    public class NgGridConfig
    {
        #region properties
        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        //html - public string aggregateTemplate { get; set; }
        //function - afterSelectionChange
        //function - beforeSelectionChange
        //html - public string checkboxCellTemplate { get; set; }
        //html - public string checkboxHeaderTemplate { get; set; }
        public IList<NgGridColumn> columnDefs { get; set; }
        //public IList data { get; set; }
        //default - public bool enableCellEdit { get; set; }
        //default - public bool enableCellSelection { get; set; }
        //public bool enableColumnResize { get; set; }
        //public bool enableColumnReordering { get; set; }
        //default - public bool enableHighlighting { get; set; }
        //public bool enablePaging { get; set; }
        //default - public bool enableRowReordering { get; set; }
        public bool enableRowSelection { get; set; }
        //public bool enableSorting { get; set; }
        public FilterConfig filterOptions { get; set; }
        //default - public uint footerRowHeight { get; set; }
        public IList<string> groups { get; set; }
        //public bool groupsCollapsedByDefault { get; set; }
        //default - public uint headerRowHeight { get; set; }
        //html - public string headerRowTemplate { get; set; }
        //default - public bool jqueryUIDraggable { get; set; }
        //default - public bool jqueryUITheme { get; set; }
        //default - public bool keepLastSelected { get; set; }
        //default - public string maintainColumnRatios { get; set; }
        public bool multiSelect { get; set; }
        public PagingConfig pagingOptions { get; set; }
        //function - public IList<string> plugins { get; set; }
        public string primaryKey { get; set; }
        //default - public uint rowHeight { get; set; }
        //html - public string rowTemplate { get; set; }
        public IList selectedItems { get; set; }
        //default - public bool selectWithCheckboxOnly { get; set; }
        //public bool showColumnMenu { get; set; }
        //default - public bool showFilter { get; set; }
        //default - public bool showFooter { get; set; }
        //default - public bool showGroupPanel { get; set; }
        public SortConfig sortInfo { get; set; }
        public bool showSelectionCheckbox { get; set; }
        //default - public ushort tabIndex { get; set; }
        //public bool useExternalSorting { get; set; }
        //default - public ushort virtualizationThreshold { get; set; }
        //function - selectedRow
        //function - selectedItem
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore InconsistentNaming
        #endregion

        #region ctor
        public NgGridConfig()
        {
            columnDefs = new List<NgGridColumn>();
            //data = new ArrayList();
            //enableColumnResize = true;
            //enableColumnReordering = true;
            //enablePaging = true;
            enableRowSelection = true;
            //enableSorting = true;
            filterOptions = new FilterConfig { useExternalFilter = true };
            groups = new List<string>();
            //groupsCollapsedByDefault = true;
            multiSelect = true;
            pagingOptions = new PagingConfig { currentPage = 1, totalServerItems = 0, pageSize = 20, pageSizes = new HashSet<ushort> { 20, 50, 100, 200, 500, 1000 } };
            primaryKey = string.Empty;
            selectedItems = new ArrayList();
            //showColumnMenu = true;
            sortInfo = new SortConfig();
            showSelectionCheckbox = true;
            //useExternalSorting = true;
        }
        #endregion
    }
}
