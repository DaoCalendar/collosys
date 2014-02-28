namespace ColloSys.Shared.NHPagination
{
    public class ColumnsDefinitions
    {
        public ColumnsDefinitions()
        {
            Width = 150;
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string CellFilter { get; set; }

        public string DisplayName { get; set; }

        public string Field { get; set; }

        public uint Width { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}