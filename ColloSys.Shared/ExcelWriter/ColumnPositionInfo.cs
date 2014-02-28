using System.Reflection;

namespace ColloSys.Shared.ExcelWriter
{
    public class ColumnPositionInfo
    {
        public ColumnPositionInfo()
        {
            IsFreezed = false;
            UseFieldNameForDisplay = false;
            WriteInExcel = true;
        }
        public uint Position { get; set; }

        public string DisplayName { get; set; }

        public string  FieldName { get; set; }

        public bool WriteInExcel { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public bool UseFieldNameForDisplay { get; set; }

        public bool IsFreezed { get; set; }
    }
}