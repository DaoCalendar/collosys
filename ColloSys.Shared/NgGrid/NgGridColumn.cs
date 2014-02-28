namespace ColloSys.Shared.NgGrid
{
    public class NgGridColumn
    {
        #region properties

        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        //function - public string aggLabelFilter { get; set; }
        public string cellClass { get; set; }
        public string cellFilter { get; set; }
        //html - public string cellTemplate { get; set; }
        public string displayName { get; set; }
        //future - public string editableCellTemplate { get; set; }
        //public bool enableCellEdit { get; set; }
        public string field { get; set; }
        //custom - public string type { get; set; }
        //public bool groupable { get; set; }
        //public string headerCellTemplate {get; set; }
        //public string headerClass {get; set;}
        public string sortDirection { get; set; } // sort direction saved in case sorted
        public uint index { get; set; } // should be saved in case position is changed
        public ushort maxWidth { get; set; }
        public ushort minWidth { get; set; }
        public bool pinned { get; set; }
        //public bool pinnable { get; set; }
        //public bool resizable { get; set; }
        //public bool sortable { get; set; }
        //function - public string sortFn { get; set; }
        //public virtual bool sortFn { get; set; }
        public bool visible { get; set; }
        public ushort width { get; set; }
        public JsTypes cellType { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore InconsistentNaming

        #endregion

        #region ctor

        public NgGridColumn()
        {
            minWidth = 100;
            maxWidth = 400;
            visible = true;
            width = 150;
        }

        #endregion
    }

    public enum JsTypes
    {
        Text,
        Amount,
        Number,
        Date,
        DateTime,
        Bool,
        Enum
    }

}