using System.Collections.Generic;
using ColloSys.DataLayer.Enumerations;
using Newtonsoft.Json.Linq;

namespace UserInterfaceAngular.NgGrid
{
    public class NgGridOptions
    {
        public NgGridOptions()
        {
            columnDefs = new List<ColumnDef>();
            CommandButton = new CommandButton();
            enableColumnResize = true;
            multiSelect = false;
        }

        private IEnumerable<JObject> _data;
        public IEnumerable<JObject> data { get { return _data; } set { _data = value; } }

        //public bool showGroupPanel { get; set; }
        //public bool enablePinning { get; set; }
        //public string headerRowTemplate { get; set; }

        public bool enableColumnResize { get; set; }

        public bool multiSelect { get; set; }

        public bool showSelectionCheckbox { get; set; }

        public List<ColumnDef> columnDefs;

        public CommandButton CommandButton { get; set; }

        //public void SetData(object obj)
        //{
        //    var serializer = new JavaScriptSerializer();

        //    _data = serializer.Serialize(obj);
        //}
    }


    public class ColumnDef
    {
        public ColumnDef()
        {
            width = 100;
            readOnly = false;
        }

        public string field { get; set; }

        public string displayName { get; set; }

        public uint width { get; set; }

        public bool readOnly { get; set; }

        public ColloSysEnums.HtmlInputType InputType { get; set; }

        public IEnumerable<string> dropDownValues { get; set; }


        //public bool pinned { get; set; }
    }   

    public class CommandButton
    {
        public bool AddNew { get; set; }

        public bool Edit { get; set; }

        public bool View { get; set; }

        public bool Delete { get; set; }

    }
}