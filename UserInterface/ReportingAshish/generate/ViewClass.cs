using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Domain;

namespace UserInterfaceAngular.Areas.Reporting.ViewModels
{
    public class ViewClass
    {
    }

    public class ReportWrapper
    {
        public virtual string totalRecords { get; set; }
        public virtual IEnumerable<dynamic> DataList { get; set; }
        public virtual IList<Columns> Columns { get; set; }
        public virtual IList<GridOptions> GridOptions { get; set; }
    }

    public class ReportFilterWrapper
    {
        public virtual string totalRecords { get; set; }
        public virtual IEnumerable<dynamic> DataList { get; set; }
        public virtual IList<Columns> Columns { get; set; }
        public virtual IList<GridOptions> GridOptions { get; set; }
    }

    public class ExportColumns
    {
        public virtual string[] columns { get; set; }
    }
    
    public class Filter
    {
        public string modelName { get; set; } //class name e.g. employee
        public string propertyName { get; set; } // propertyname e.g. employeeid, name etc
        public string propertytype { get; set; }
        public string operatortype { get; set; }
        public string val1 { get; set; }
        public string val2 { get; set; }
    }

    public class ClientDataClass
    { 
    public FilterClass [] filterClass {get;set;}
    public ColFilter[] colFilter { get; set; }
    }

    public class FilterClass
    {
        public string modelName { get; set; } //class name e.g. employee
        public string propertyName { get; set; } // propertyname e.g. employeeid, name etc
        public string propertytype { get; set; }
        public string operatortype { get; set; }
        public string val1 { get; set; }
        public string val2 { get; set; }
        public string index { get; set; }
    }

    public class ColFilter
    {
        public string modelName { get; set; } //class name e.g. employee
        public string operatortype { get; set; }
        public string val1 { get; set; }
        public string val2 { get; set; }
        public string index { get; set; }
    }

    public class Columns
    {
        public virtual string field { get; set; }
        public virtual string type { get; set; }
        public virtual string aggLabelFilter { get; set; }
        public virtual string cellClass { get; set; }
        public virtual string cellFilter { get; set; }
        public virtual string cellTemplate { get; set; }
        public virtual string displayName { get; set; }
        public virtual string editableCellTemplate { get; set; }
        public virtual bool enableCellEdit { get; set; }
        public virtual bool groupable { get; set; }
        public virtual bool pinnable { get; set; }
        public virtual bool resizable { get; set; }
        public virtual bool sortable { get; set; }
        //public virtual bool sortFn { get; set; }
        public virtual string width { get; set; }
        public virtual string maxWidth { get; set; }
        public virtual string minWidth { get; set; }
        public virtual string headerClass { get; set; }
        public virtual string headerCellTemplate { get; set; }
        public virtual bool visible { get; set; }

    }

    public class GridOptions
    {
        public virtual string aggregateTemplate { get; set; }
        public virtual string checkboxCellTemplate { get; set; }
        public virtual string checkboxHeaderTemplate { get; set; }
        public virtual bool enableCellEdit { get; set; }
        public virtual bool enableCellSelection { get; set; }
        public virtual bool enableColumnResize { get; set; }
        public virtual bool enableColumnReordering { get; set; }
        public virtual bool enableHighlighting { get; set; }
        public virtual bool enablePaging { get; set; }
        public virtual bool enableRowReordering { get; set; }
        public virtual bool enableRowSelection { get; set; }
        public virtual bool enableSorting { get; set; }
        public virtual string footerRowHeight { get; set; }
        public virtual string plugins { get; set; }
        public virtual string groups { get; set; }
        public virtual bool groupsCollapsedByDefault { get; set; }
        public virtual string headerRowHeight { get; set; }
        public virtual string headerRowTemplate { get; set; }
        public virtual bool jqueryUIDraggable { get; set; }
        public virtual bool jqueryUITheme { get; set; }
        public virtual bool keepLastSelected { get; set; }
        public virtual bool multiSelect { get; set; }
        public virtual string pagingOptions { get; set; }
        public virtual string rowHeight { get; set; }
        public virtual string rowTemplate { get; set; }
        public virtual bool selectWithCheckboxOnly { get; set; }
        public virtual bool showColumnMenu { get; set; }
        public virtual bool showFilter { get; set; }
        public virtual bool showFooter { get; set; }
        public virtual bool showGroupPanel { get; set; }
        public virtual bool showSelectionCheckbox { get; set; }
        public virtual string tabIndex { get; set; }
    }

}