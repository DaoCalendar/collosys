using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using UserInterfaceAngular.Areas.Reporting.ViewModels;

namespace ColloSys.UserInterface.Areas.Reporting.ViewModels
{
    public class commonUtility
    {
        public static string dateformat = "yyyy-MM-ddTHH:mm:ssZ";
        public static void BuildHql(IEnumerable<FilterClass> filter, Hashtable _filters)
        {
            foreach (var t in filter)
            {
                if (getTypes.getpropertyType(t.propertytype).ToUpper() == "UINT32" || getTypes.getpropertyType(t.propertytype).ToUpper() == "UINT64" || getTypes.getpropertyType(t.propertytype).ToUpper() == "DECIMAL" || getTypes.getpropertyType(t.propertytype).ToUpper() == "DOUBLE")
                {
                    switch (t.operatortype.Trim().ToUpper())
                    {
                        case "EQUALS":
                            AddtoHql(t.propertyName, t.propertyName + " = " + t.val1, _filters);
                            break;
                        case "NOT EQUALS":
                            AddtoHql(t.propertyName, t.propertyName + " <> " + t.val1, _filters);
                            break;
                        case "IS LESS THAN":
                            AddtoHql(t.propertyName, t.propertyName + " < " + t.val1, _filters);
                            break;
                        case "IS LESS THAN EQUALS":
                            AddtoHql(t.propertyName, t.propertyName + " <= " + t.val1, _filters);
                            break;
                        case "IS GREATER THAN":
                            AddtoHql(t.propertyName, t.propertyName + " > " + t.val1, _filters);
                            break;
                        case "IS GREATER THAN EQUALS":
                            AddtoHql(t.propertyName, t.propertyName + " >= " + t.val1, _filters);
                            break;
                        case "IN BETWEEN":
                            AddtoHql(t.propertyName, t.propertyName + " >= " + t.val1 + " AND " + t.propertyName + " <= " + t.val2, _filters);
                            break;
                        case "IS NOT LESS THAN":
                            AddtoHql(t.propertyName, t.propertyName + " >= " + t.val1, _filters);
                            break;
                        case "IS NOT GREATER THAN":
                            AddtoHql(t.propertyName, t.propertyName + " <= " + t.val1, _filters);
                            break;
                        case "IS NOT BETWEEN":
                            AddtoHql(t.propertyName, t.propertyName + " NOT BETWEEN " + t.val1 + " AND " + t.val2, _filters);
                            break;
                        case "EQUALS FIELD":
                            AddtoHql(t.propertyName + t.val1 + "=", t.propertyName + " = " + t.val1, _filters);
                            break;
                        case "NOT EQUALS FIELD":
                            AddtoHql(t.propertyName + t.val1 + "<>", t.propertyName + " <> " + t.val1, _filters);
                            break;
                        case "IS LESS THAN FIELD":
                            AddtoHql(t.propertyName + t.val1 + "<", t.propertyName + " < " + t.val1, _filters);
                            break;
                        case "IS LESS THAN EQUALS FIELD":
                            AddtoHql(t.propertyName + t.val1 + "<=", t.propertyName + " <= " + t.val1, _filters);
                            break;
                        case "IS GREATER THAN FIELD":
                            AddtoHql(t.propertyName + t.val1 + ">", t.propertyName + " > " + t.val1, _filters);
                            break;
                        case "IS GREATER THAN EQUALS FIELD":
                            AddtoHql(t.propertyName + t.val1 + ">=", t.propertyName + " >= " + t.val1, _filters);
                            break;
                        case "IS NOT LESS THAN FIELD":
                            AddtoHql(t.propertyName + t.val1 + "NOT>=", t.propertyName + " >= " + t.val1, _filters);
                            break;
                        case "IS NOT GREATER THAN FIELD":
                            AddtoHql(t.propertyName + t.val1 + "NOT<=", t.propertyName + " <= " + t.val1, _filters);
                            break;
                    }
                }

                else if (getTypes.getpropertyType(t.propertytype).ToUpper() == "STRING" || getTypes.getpropertyType(t.propertytype).ToUpper() == "ENUM")
                {
                    switch (t.operatortype.Trim().ToUpper())
                    {
                        case "EQUALS ENUM":
                            AddtoHql(t.propertyName, t.propertyName + " = '" + t.val1 + "'", _filters);
                            break;
                        case "NOT EQUALS ENUM":
                            AddtoHql(t.propertyName, t.propertyName + " <> '" + t.val1 + "'", _filters);
                            break;
                        case "STARTS WITH":
                            AddtoHql(t.propertyName, t.propertyName + " like '" + t.val1 + "%'", _filters);
                            break;
                        case "ENDS WITH":
                            AddtoHql(t.propertyName, t.propertyName + " like '%" + t.val1 + "'", _filters);
                            break;
                        case "CONTAINS":
                            AddtoHql(t.propertyName, t.propertyName + " like '%" + t.val1 + "%'", _filters);
                            break;
                        case "NOT CONTAINS":
                            AddtoHql(t.propertyName, t.propertyName + " Not like '%" + t.val1 + "%'", _filters);
                            break;
                        case "NOT IN MULTIPLE":
                            AddtoHql(t.propertyName, t.propertyName + " NOT IN ('" + t.val1.Replace(",","','") + "')", _filters);
                            break;
                        case "IN MULTIPLE":
                            AddtoHql(t.propertyName, t.propertyName + " IN ('" + t.val1.Replace(",", "','") + "')", _filters);
                            break;
                    }
                }
                else if (getTypes.getpropertyType(t.propertytype).ToUpper() == "BOOLEAN")
                {
                    AddtoHql(t.propertyName, t.propertyName + " = " + t.operatortype, _filters);
                }
                else if (getTypes.getpropertyType(t.propertytype).ToUpper() == "DATETIME")
                {
                    switch (t.operatortype.Trim().ToUpper())
                    {
                        case "EQUALS":
                            AddtoHql(t.propertyName, t.propertyName + " = '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "'", _filters);
                            break;
                        case "NOT EQUALS":
                            AddtoHql(t.propertyName, t.propertyName + " <> '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "'", _filters);
                            break;
                        case "IS LESS THAN":
                            AddtoHql(t.propertyName, t.propertyName + " < '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "'", _filters);
                            break;
                        case "IS LESS THAN EQUALS":
                            AddtoHql(t.propertyName, t.propertyName + " <= '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "'", _filters);
                            break;
                        case "IS GREATER THAN":
                            AddtoHql(t.propertyName, t.propertyName + " > '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "'", _filters);
                            break;
                        case "IS GREATER THAN EQUALS":
                            AddtoHql(t.propertyName, t.propertyName + " >= '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "'", _filters);
                            break;
                        case "IN BETWEEN":
                            AddtoHql(t.propertyName, t.propertyName + " BETWEEN '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "' AND '" + Convert.ToDateTime(t.val2).ToString(dateformat) + "'", _filters);
                            break;
                        case "IS NOT LESS THAN":
                            AddtoHql(t.propertyName, t.propertyName + " >= '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "'", _filters);
                            break;
                        case "IS NOT GREATER THAN":
                            AddtoHql(t.propertyName, t.propertyName + " <= '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "'", _filters);
                            break;
                        case "IS NOT BETWEEN":
                            AddtoHql(t.propertyName, t.propertyName + " NOT BETWEEN '" + Convert.ToDateTime(t.val1).ToString(dateformat) + "' AND '" + Convert.ToDateTime(t.val2).ToString(dateformat) + "'", _filters);
                            break;
                        case "EQUALS FIELD":
                            AddtoHql(t.propertyName + t.val1 + "=", t.propertyName + " = " + t.val1, _filters);
                            break;
                        case "NOT EQUALS FIELD":
                            AddtoHql(t.propertyName + t.val1 + "<>", t.propertyName + " <> " + t.val1, _filters);
                            break;
                        case "IS LESS THAN FIELD":
                            AddtoHql(t.propertyName + t.val1 + "<", t.propertyName + " < " + t.val1, _filters);
                            break;
                        case "IS LESS THAN EQUALS FIELD":
                            AddtoHql(t.propertyName + t.val1 + "<=", t.propertyName + " <= " + t.val1, _filters);
                            break;
                        case "IS GREATER THAN FIELD":
                            AddtoHql(t.propertyName + t.val1 + ">", t.propertyName + " > " + t.val1, _filters);
                            break;
                        case "IS GREATER THAN EQUALS FIELD":
                            AddtoHql(t.propertyName + t.val1 + ">=", t.propertyName + " >= " + t.val1, _filters);
                            break;
                        case "IS NOT LESS THAN FIELD":
                            AddtoHql(t.propertyName + t.val1 + "NOT>=", t.propertyName + " >= " + t.val1, _filters);
                            break;
                        case "IS NOT GREATER THAN FIELD":
                            AddtoHql(t.propertyName + t.val1 + "NOT<=", t.propertyName + " <= " + t.val1, _filters);
                            break;
                    }
                }
            }
        }

     
        public static void AddtoHql(string key, string value, Hashtable _filters)
        {
            if (_filters.Contains(key))
            {
                _filters[key] = (string)(_filters[key]) + " OR " + value;
            }
            else
            {
                _filters.Add(key, value);
            }

        }

        public static string BuildFilter(Hashtable _filter)
        {
            var strFilters = string.Empty;
            foreach (string key in _filter.Keys)
            {
                if (strFilters == string.Empty) strFilters = " WHERE " + (string)(_filter[key]);
                else strFilters += " AND " + (string)(_filter[key]);
            }

            return strFilters;
        }


        public static Type GetType(string typeName)
        {
            Assembly assembly = Assembly.Load("ColloSys.DataLayer");
            Type type = assembly.GetType("ColloSys.DataLayer.Domain." + typeName);
            return type;
        }

        public static string getTableNames(string aliasName)
        {
            string tableName = string.Empty;
            switch (aliasName.ToUpper())
            {
                case "FILE DETAILS":
                    tableName = "FileDetail";
                    break;
                case "FILE COLUMNS":
                    tableName = "FileColumn";
                    break;
                case "STAKE HIERARCHY":
                    tableName = "StakeHierarchy";
                    break;
                case "CACS ACTIVITY":
                    tableName = "CacsActivity";
                    break;
                case "CREDIT CARD LINER":
                    tableName = "CLiner";
                    break;
                case "CREDIT CARD PAYMENT":
                    tableName = "CPayment";
                    break;
                case "CREDIT CARD UNBILLED":
                    tableName = "CUnbilled";
                    break;
                case "CREDIT CARD WRITEOFF":
                    tableName = "CWriteoff";
                    break;
                case "RLS LINER":
                    tableName = "RLiner";
                    break;
                case "RLS PAYMENT":
                    tableName = "RPayment";
                    break;
                case "RLS WRITEOFF":
                    tableName = "RWriteoff";
                    break;
                case "EBBS LINER":
                    tableName = "ELiner";
                    break;
                case "EBBS PAYMENT":
                    tableName = "EPayment";
                    break;
                case "EBBS WRITEOFF":
                    tableName = "EWriteoff";
                    break;
            }
            return tableName;
        }
    }
}