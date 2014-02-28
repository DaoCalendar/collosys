using ColloSys.DataLayer.Allocation;
using System;
using ColloSys.DataLayer.Enumerations;
using ColloSys.Shared.Types4Product;
using NHibernate;
using NHibernate.SqlCommand;

namespace ColloSys.AllocationService.ConditionLayer
{
    internal static class ConditionValues
    {
        /// <summary>
        /// Get column name from Allocation Subpolicy Condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string ColumnName(AllocCondition condition)
        {
            return condition.ColumnName; //relation.AllocSubpolicy.AllocSubPolicyCondition.ColumnName;
        }

        /// <summary>
        /// Get value name from Allocation Subpolicy Condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static object Value(AllocCondition condition)
        {
            return condition.Value; //relation.AllocSubpolicy.AllocSubPolicyCondition.Value;
        }

        /// <summary>
        /// Get Operator name from Allocation Subpolicy Condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ColloSysEnums.Operators Operator(AllocCondition condition)
        {
            return condition.Operator; //relation.AllocSubpolicy.AllocSubPolicyCondition.Operator;
        }

        /// <summary>
        /// Convert value to related column type
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="value"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static object ConvertValue(Type classType,
            object value, string columnName, ICriteria criteria)
        {
            string tablename, column;
            var stringList = columnName.Split('.');
            tablename = stringList[0];
            column = stringList[1];

            //var typeAnother =ClassType.GetClientDataClassTypeByTableName(tablename);
            var typeAnother =ClassType.GetType(tablename);
            if (tablename != classType.Name)
            {
                criteria.CreateCriteria(tablename, typeAnother.Name, JoinType.InnerJoin);
            }
            var propertyInfo = (tablename == classType.Name)
                                   ? classType.GetProperty(column)
                                   : typeAnother.GetProperty(column);
            var propertyType = propertyInfo.PropertyType;

            if (propertyType.IsGenericType &&
                propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            if (propertyType.IsEnum)
            {
                var enumname = propertyType.FullName.Split('+')[1];
                var enm =(ColloSysEnums.ListOfEnums) Enum.Parse(typeof (ColloSysEnums.ListOfEnums), enumname);
                switch (enm)
                {
                    case ColloSysEnums.ListOfEnums.AllocStatus:
                        value = Enum.Parse(typeof (ColloSysEnums.AllocStatus), value.ToString());
                        break;
                    case ColloSysEnums.ListOfEnums.NoAllocResons:
                        value = Enum.Parse(typeof(ColloSysEnums.NoAllocResons), value.ToString());
                        break;
                    case ColloSysEnums.ListOfEnums.DelqAccountStatus:
                        value = Enum.Parse(typeof(ColloSysEnums.DelqAccountStatus), value.ToString());
                        break;
                    case ColloSysEnums.ListOfEnums.DelqFlag:
                        value = Enum.Parse(typeof(ColloSysEnums.DelqFlag), value.ToString());
                        break;
                    case ColloSysEnums.ListOfEnums.AllocationType:
                        value = Enum.Parse(typeof(ColloSysEnums.AllocationType), value.ToString());
                        break;
                    case ColloSysEnums.ListOfEnums.UsedFor:
                        value = Enum.Parse(typeof(ColloSysEnums.UsedFor), value.ToString());
                        break;
                    case ColloSysEnums.ListOfEnums.Products:
                        value = Enum.Parse((typeof (ScbEnums.Products)), value.ToString());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (propertyType.FullName)
            {
                case "System.UInt16":
                    value = Convert.ToUInt16(value);
                    break;

                case "System.UInt32":
                    value = Convert.ToUInt32(value);
                    break;

                case "System.UInt64":
                    value = Convert.ToUInt64(value);
                    break;

                case "System.Int16":
                    value = Convert.ToInt16(value);
                    break;

                case "System.Int32":
                    value = Convert.ToInt32(value);
                    break;

                case "System.Int64":
                    value = Convert.ToInt64(value);
                    break;

                case "System.Decimal":
                    value = Convert.ToDecimal(value);
                    break;

                case "System.Guid":
                    value = Guid.Parse(value.ToString());
                    break;

                case "System.DateTime":
                    value = Convert.ToDateTime(value);
                    break;

                case "System.Char":
                    value = Convert.ToChar(value);
                    break;

                case "System.Byte":
                    value = Convert.ToByte(value);
                    break;

                case "System.Boolean":
                    value = Convert.ToBoolean(value);
                    break;
                    
            }
            return value;
        }
    }
}
