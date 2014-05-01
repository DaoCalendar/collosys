using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.Shared.Types4Product;
using UserInterfaceAngular.NgGrid;

namespace ColloSys.UserInterface.Shared
{
    public class SharedViewModel
    {
        public static IEnumerable<ColumnDef> ConditionColumns(ScbEnums.Products products, ScbEnums.Category category)
        {
            IList<ColumnDef> columns = new List<ColumnDef>();
            var propertyList = new List<PropertyInfo>();

            var classType = ClassType.GetTypeByProductCategoryForAlloc(products, category);
            propertyList.AddRange(GetPropertyList(classType));
            propertyList.ForEach(c => columns.Add(new ColumnDef
            {
                field = classType.Name + "." + c.Name,
                displayName = classType.Name + "." + c.Name,
                InputType = GetHtmlInputType(c.PropertyType),
                dropDownValues = !c.PropertyType.IsEnum ? null : Enum.GetNames(c.PropertyType)
            }));

            var gpincodeprops = new List<PropertyInfo>();
            gpincodeprops.AddRange(GetPropertyList(typeof(GPincode)));
            gpincodeprops.ForEach(c => columns.Add(new ColumnDef
            {
                field = "GPincode." + c.Name,
                displayName = "Pincode"+c.Name,
                InputType = GetHtmlInputType(c.PropertyType),
                dropDownValues = !c.PropertyType.IsEnum ? null : Enum.GetNames(c.PropertyType)
            }));

            return columns.OrderBy(x=>x.displayName).ToList();
        }

        public static IEnumerable<ColumnDef> BillingServiceConditionColumns()
        {
            IList<ColumnDef> columns = new List<ColumnDef>();

            var propertyList = new List<PropertyInfo>();
            var custBillType = typeof(CustBillViewModel);
            propertyList.AddRange(GetPropertyList(custBillType));
            propertyList.ForEach(c => columns.Add(new ColumnDef
            {
                field = custBillType.Name + "." + c.Name,
                displayName = "Customer." + c.Name,
                InputType = GetHtmlInputType(c.PropertyType),
                dropDownValues = !c.PropertyType.IsEnum ? null : Enum.GetNames(c.PropertyType)
            }));

            var gPincodeType = typeof(GPincode);
            var gpincodeprops = new List<PropertyInfo>();
            gpincodeprops.AddRange(GetPropertyList(gPincodeType));
            gpincodeprops.ForEach(c => columns.Add(new ColumnDef
            {
                field = gPincodeType.Name + "." + c.Name,
                displayName = "Pincode" + "." + c.Name,
                InputType = GetHtmlInputType(c.PropertyType),
                dropDownValues = !c.PropertyType.IsEnum ? null : Enum.GetNames(c.PropertyType)
            }));

            //var stkhBillType = typeof(StkhBillViewModel);
            //var stkhBillprops = new List<PropertyInfo>();
            //stkhBillprops.AddRange(GetPropertyList(stkhBillType));
            //stkhBillprops.ForEach(c => columns.Add(new ColumnDef
            //{
            //    field = stkhBillType.Name + "." + c.Name,
            //    displayName = "Stakeholder." + c.Name,
            //    InputType = GetHtmlInputType(c.PropertyType),
            //    dropDownValues = !c.PropertyType.IsEnum ? null : Enum.GetNames(c.PropertyType)
            //}));

            return columns.OrderBy(x=>x.displayName).ToList();
        }

        private static ColloSysEnums.HtmlInputType GetHtmlInputType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {

                var nullableConverter = new NullableConverter(type);
                type = nullableConverter.UnderlyingType;
            }

            if (type.IsEnum)
            {
                return ColloSysEnums.HtmlInputType.dropdown;
            }

            switch (type.FullName)
            {
                case "System.String":
                case "System.Char":
                    return ColloSysEnums.HtmlInputType.text;
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Decimal":
                case "System.Double":
                    return ColloSysEnums.HtmlInputType.number;
                case "System.DateTime":
                    return ColloSysEnums.HtmlInputType.date;
                case "System.Boolean":
                    return ColloSysEnums.HtmlInputType.checkbox;
                default:
                    throw new Exception("Type is not valid");
            }
        }
        private static IEnumerable<PropertyInfo> GetPropertyList(Type p)
        {
            var propertyList = p.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return from property in propertyList
                   let type = property.PropertyType
                   where type.BaseType == null || type.BaseType != typeof(Entity)
                   where type.BaseType == null || type.BaseType.BaseType == null || type.BaseType.BaseType != typeof(Entity)
                   where !type.IsGenericType || type.GetGenericTypeDefinition() == typeof(Nullable<>)
                   where !typeof(Entity).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Select(c => c.Name).Contains(property.Name)
                   select property;
        }
    }
}

//private static IEnumerable<Type> ProductClassTypes(string products, ScbEnums.Category category)
//{
//    if (category == ScbEnums.Category.Liner)
//    {
//        switch (products)
//        {
//            case "CreditCard":
//                return new[] { typeof(CLiner) };
//            case "AutoOd":
//            case "SmartCredit":
//                return new[] { typeof(ELiner) };
//            case "BFS":
//            case "BusinessLoan":
//            case "Mortgage":
//            case "PersonalLoan":
//            case "SME":
//                return new[] { typeof(RLiner) };
//            default:
//                throw new Exception("Invalid Product type");
//        }
//    }

//    if (category == ScbEnums.Category.WriteOff)
//    {
//        switch (products)
//        {
//            case "CreditCard":
//                return new[] { typeof(CWriteoff) };
//            case "AutoOd":
//            case "SmartCredit":
//                return new[] { typeof(EWriteoff) };
//            case "BFS":
//            case "BusinessLoan":
//            case "Mortgage":
//            case "PersonalLoan":
//            case "SME":
//                return new[] { typeof(RWriteoff) };
//            default:
//                throw new Exception("Invalid Product type");
//        }
//    }

//    throw new Exception("Invalid Category type");
//}
