#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.Shared.NgGrid;
using Itenso.TimePeriod;
using NHibernate;
using NHibernate.Criterion;

#endregion

namespace ColloSys.Shared.NHPagination
{
    public static class NhCriteriaGenerator
    {
        #region to icriteria
        public static ICriteria GetExecutableCriteria(DetachedCriteria detachedCriteria)
        {
            var session = SessionManager.GetCurrentSession();
            return detachedCriteria.GetExecutableCriteria(session);
        }
        #endregion

        #region ordering

        public static DetachedCriteria AddOrdering(DetachedCriteria criteria, string property, string direction)
        {
            if (criteria == null) return null;
            if (String.IsNullOrWhiteSpace(property)) return criteria;
            var directionAsc = String.IsNullOrWhiteSpace(direction) || direction.ToUpperInvariant() == "ASC";
            return criteria.AddOrder(new Order(property, directionAsc));
        }

        #endregion

        #region pagination

        public static ulong GetTotalRowCount(DetachedCriteria detachedCriteria)
        {
            var session = SessionManager.GetCurrentSession();
            var criteria = detachedCriteria.GetExecutableCriteria(session);
            var crit = CriteriaTransformer.TransformToRowCount(criteria);
            return (ulong)crit.FutureValue<Int32>().Value;
        }

        public static uint GetTotalPageCount(ulong totalRows, uint pageSize)
        {
            var pages = (uint)(totalRows / pageSize);
            if ((pages * pageSize) < totalRows) pages++;
            return pages;
        }

        public static DetachedCriteria AddPaging(DetachedCriteria criteria, uint pageNo = 1, uint pageSize = 100)
        {
            if (pageNo == 0) pageNo = 1;
            if (pageSize == 0) pageSize = 100;
            var firstresult = (pageNo - 1) * pageSize;
            return criteria.SetFirstResult((int)firstresult).SetMaxResults((int)pageSize);
        }

        #endregion

        #region filtering
        private static object StringToType(string value, Type propertyType)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var realType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            if (realType.IsEnum) return Enum.Parse(realType, value);
            return Convert.ChangeType(value, realType, CultureInfo.InvariantCulture);
        }

        private static PropertyInfo GetPropertyType(Type classType, string fieldName)
        {
            PropertyInfo property = null;
            var childType = classType;
            foreach (var part in fieldName.Split('.'))
            {
                var info = childType.GetProperty(part);
                if (info == null) return property;
                property = info;
                childType = info.GetType();
            }

            return property;
        }

        public static DetachedCriteria AddRelativeFiltering(DetachedCriteria detachedCriteria, Type objType,
                                                            IEnumerable<FilterParams> filterConfig)
        {
            var filters2Add = filterConfig.Where(x => x.FilterGroup == FilterGroup.Runtime && !String.IsNullOrWhiteSpace(x.FieldName)).ToList();
            if (filters2Add.Count <= 0) return detachedCriteria;

            // for each key, add filter by type
            foreach (var filter in filters2Add)
            {
                var property = GetPropertyType(objType, filter.FieldName);
                if (property == null) continue;
                var name = objType.Name + "." + filter.FieldName;

                switch (filter.Operator)
                {
                    case FilterOperators.Today:
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), DateTime.Today));
                        break;
                    case FilterOperators.Tomorrow:
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), DateTime.Today.AddDays(1)));
                        break;
                    case FilterOperators.Yesterday:
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), DateTime.Today.AddDays(-1)));
                        break;
                    case FilterOperators.CurrentWeek:
                        var week = new Week(DateTime.Today);
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), week.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), week.End));
                        break;
                    case FilterOperators.CurrentMonth:
                        var month = new Month(DateTime.Today);
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), month.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), month.End));
                        break;
                    case FilterOperators.CurrentQuarter:
                        var quarter = new Quarter(DateTime.Today);
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), quarter.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), quarter.End));
                        break;
                    case FilterOperators.CurrentYear:
                        var year = new Year(DateTime.Today);
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), year.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), year.End));
                        break;
                    case FilterOperators.LastWeek:
                        var pweek = new Week(DateTime.Today).GetPreviousWeek();
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), pweek.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), pweek.End));
                        break;
                    case FilterOperators.LastMonth:
                        var lmonth = new Month(DateTime.Today).GetPreviousMonth();
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), lmonth.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), lmonth.End));
                        break;
                    case FilterOperators.LastQuarter:
                        var lquarter = new Quarter(DateTime.Today).GetPreviousQuarter();
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), lquarter.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), lquarter.End));
                        break;
                    case FilterOperators.LastYear:
                        var pyear = new Year(DateTime.Today).GetPreviousYear();
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), pyear.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), pyear.End));
                        break;
                    case FilterOperators.NextWeek:
                        var nweek = new Week(DateTime.Today).GetNextWeek();
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), nweek.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), nweek.End));
                        break;
                    case FilterOperators.NextMonth:
                        var nmonth = new Month(DateTime.Today).GetNextMonth();
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), nmonth.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), nmonth.End));
                        break;
                    case FilterOperators.NextQuarter:
                        var nquarter = new Quarter(DateTime.Today).GetNextQuarter();
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), nquarter.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), nquarter.End));
                        break;
                    case FilterOperators.NextYear:
                        var nyear = new Year(DateTime.Today).GetNextYear();
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), nyear.Start));
                        detachedCriteria = detachedCriteria.Add(Restrictions.Eq(Projections.Property(name), nyear.End));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("filterConfig");
                }
            }
            return detachedCriteria;
        }

        public static DetachedCriteria AddValueFiltering(DetachedCriteria detachedCriteria, Type objType,
                                                    IEnumerable<FilterParams> filterConfig)
        {
            var filters2Add = filterConfig.Where(x => x.FilterGroup == FilterGroup.Value && !String.IsNullOrWhiteSpace(x.FieldName)).ToList();
            if (filters2Add.Count <= 0) return detachedCriteria;

            // for each key, add filter by type
            foreach (var filter in filters2Add)
            {
                var property = GetPropertyType(objType, filter.FieldName);
                if (property == null) continue;
                var value = StringToType(filter.FilterValue, property.PropertyType);
                var name = objType.Name + "." + filter.FieldName;

                switch (filter.Operator)
                {
                    case FilterOperators.Equal:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.Eq(Projections.Property(name), value));
                        break;
                    case FilterOperators.NotEqual:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.Not(
                                Restrictions.Eq(Projections.Property(name), value)));
                        break;
                    case FilterOperators.LessThan:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.Lt(Projections.Property(name), value));
                        break;
                    case FilterOperators.LessThanEqual:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.Le(Projections.Property(name), value));
                        break;
                    case FilterOperators.GreaterThan:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.Gt(Projections.Property(name), value));
                        break;
                    case FilterOperators.GreaterThanEqual:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.Ge(Projections.Property(name), value));
                        break;
                    case FilterOperators.BeginsWith:
                        detachedCriteria.Add(Restrictions.InsensitiveLike(
                            Projections.Cast(NHibernateUtil.String, Projections.Property(name)),
                            value.ToString(), MatchMode.Start));
                        break;
                    case FilterOperators.EndsWith:
                        detachedCriteria.Add(Restrictions.InsensitiveLike(
                            Projections.Cast(NHibernateUtil.String, Projections.Property(name)),
                            value.ToString(), MatchMode.End));
                        break;
                    case FilterOperators.Contains:
                        detachedCriteria.Add(Restrictions.InsensitiveLike(
                            Projections.Cast(NHibernateUtil.String, Projections.Property(name)),
                            value.ToString(), MatchMode.Anywhere));
                        break;
                    case FilterOperators.NotContains:
                        detachedCriteria.Add(Restrictions.Not(Restrictions.InsensitiveLike(
                            Projections.Cast(NHibernateUtil.String, Projections.Property(name)),
                            value.ToString(), MatchMode.Anywhere)));
                        break;
                    case FilterOperators.IsInList:
                        if (!property.PropertyType.IsEnum) break;
                        var values2 = filter.FilterValueList.Select(value1 => StringToType(value1, property.PropertyType))
                                           .Where(typedValue => typedValue != null);
                        detachedCriteria = detachedCriteria.Add(Restrictions.In(Projections.Property(name), values2.ToArray()));
                        break;
                    case FilterOperators.NotInList:
                        if (!property.PropertyType.IsEnum) break;
                        var values = filter.FilterValueList.Select(value1 => StringToType(value1, property.PropertyType))
                                           .Where(typedValue => typedValue != null);
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.Not(
                                Restrictions.In(Projections.Property(name), values.ToArray())));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("filterConfig");
                }
            }
            return detachedCriteria;
        }

        public static DetachedCriteria AddFieldFiltering(DetachedCriteria detachedCriteria, Type objType,
                                                         IEnumerable<FilterParams> filterConfig)
        {
            var filters2Add = filterConfig.Where(x => x.FilterGroup == FilterGroup.Field
                                                      && !String.IsNullOrWhiteSpace(x.FilterValue)
                                                      && !String.IsNullOrWhiteSpace(x.FieldName))
                                          .ToList();
            if (filters2Add.Count <= 0) return detachedCriteria;

            foreach (var filter in filters2Add)
            {
                var leftprop = objType.GetProperty(filter.FilterValue);
                var rightprop = objType.GetProperty(filter.FieldName);
                if (leftprop == null || rightprop == null) continue;

                var lhsName = objType.Name + "." + filter.FieldName;
                var rhsName = objType.Name + "." + filter.FilterValue;

                switch (filter.Operator)
                {
                    case FilterOperators.Equal:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.EqProperty(Projections.Property(lhsName),
                                                    Projections.Property(rhsName)));
                        break;
                    case FilterOperators.NotEqual:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.Not(
                                Restrictions.EqProperty(Projections.Property(lhsName),
                                                        Projections.Property(rhsName))));
                        break;
                    case FilterOperators.LessThan:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.LtProperty(Projections.Property(lhsName),
                                                    Projections.Property(rhsName)));
                        break;
                    case FilterOperators.LessThanEqual:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.LeProperty(Projections.Property(lhsName),
                                                    Projections.Property(rhsName)));
                        break;
                    case FilterOperators.GreaterThan:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.GtProperty(Projections.Property(lhsName),
                                                    Projections.Property(rhsName)));
                        break;
                    case FilterOperators.GreaterThanEqual:
                        detachedCriteria = detachedCriteria.Add(
                            Restrictions.GeProperty(Projections.Property(lhsName),
                                                    Projections.Property(rhsName)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("filterConfig");
                }

            }
            return detachedCriteria;
        }
        #endregion
    }
}


//public static DetachedCriteria AddOrdering(DetachedCriteria criteria, string property, bool directionAsc)
//{
//    if (criteria == null) return null;
//    if (String.IsNullOrWhiteSpace(property)) return criteria;
//    return criteria.AddOrder(new Order(property, directionAsc));
//}



//public static void AddFiltering(DetachedCriteria detachedCriteria, FilterConfig filterConfig)
//{
//    // there must be atleast one filter
//    var filters = filterConfig.filterText.Split(';');
//    if (filters.Length <= 0) return;

//    //get props with only 2 params (fieldName & filterValue)
//    var props = filters.Select(filter => filter.Split(':'))
//                       .Where(keyvalue => keyvalue.Length == 2)
//                       .ToDictionary(keyvalue => keyvalue[0].Trim(), keyvalue => keyvalue[1].Trim());
//    if (props.Count <= 0) return;

//    foreach (var prop in props)
//    {
//        detachedCriteria.Add(Restrictions.InsensitiveLike(
//            Projections.Cast(NHibernateUtil.String, Projections.Property(prop.Key)),
//            prop.Value,
//            MatchMode.Anywhere));
//    }
//}


// convert to json
//var json = JObject.FromObject(props);
//var type = gridQueryParams.GetCriteriaType();
//var filterobj = json.ToObject(type);
//var example = Example.Create(filterobj).ExcludeNone().ExcludeNulls().ExcludeZeroes();
//var excludeprops = type.GetProperties().Where(x => !props.Keys.Contains(x.Name))
//                       .Select(x => x.Name)
//                       .ToList();
//foreach (var excludeprop in excludeprops)
//{
//    example.ExcludeProperty(excludeprop);
//}
//detachedCriteria.Add(example);

//private static object GetPropertyValue(Object classObj, string fieldName)
//{
//    foreach (var part in fieldName.Split('.'))
//    {
//        if (classObj == null) { return null; }

//        var info = classObj.GetType().GetProperty(part);
//        if (info == null) { return null; }

//        classObj = info.GetValue(classObj, null);
//    }
//    return classObj;
//}


