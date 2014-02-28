using System.Collections.Generic;

namespace ColloSys.Shared.NgGrid
{
    public class FilterConfig
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string filterText { get; set; }
        public bool useExternalFilter { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore InconsistentNaming

        public FilterConfig()
        {
            filterText = string.Empty;
            useExternalFilter = false;
        }
    }

    public enum FilterGroup
    {
        Value,
        Field,
        Runtime
    } 

    public class FilterParams
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string FieldName { get; set; }
        public JsTypes FieldType { get; set; }
        public FilterOperators Operator { get; set; }
        public string FilterValue { get; set; }
        public IList<string> FilterValueList { get; set; }
        public FilterGroup FilterGroup { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global

        public FilterParams()
        {
            FieldName = string.Empty;
            FieldType = JsTypes.Text;
            FilterValue = string.Empty;
            Operator = FilterOperators.Contains;
            FilterGroup = FilterGroup.Value;
            FilterValueList = new List<string>();
        }
    }

    public enum FilterOperators
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,
        BeginsWith,
        EndsWith,
        Contains,
        NotContains,
        Today,
        Tomorrow,
        Yesterday,
        CurrentWeek,
        CurrentMonth,
        CurrentQuarter,
        CurrentYear,
        LastWeek,
        LastMonth,
        LastQuarter,
        LastYear,
        NextWeek,
        NextMonth,
        NextQuarter,
        NextYear,
        IsInList,
        NotInList
    }
}