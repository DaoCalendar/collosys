#region references

using System;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.Shared.DbLayer;

#endregion

namespace ColloSys.Shared.Types4Product
{
    public static class ClassType
    {
        #region getType by product/category

        public static Type GetTypeBySystemCategory(ScbEnums.ScbSystems systems, ScbEnums.Category category)
        {
            if (category == ScbEnums.Category.Activity || systems == ScbEnums.ScbSystems.CACS)
            {
                return typeof(CacsActivity);
            }

            switch (systems)
            {
                case ScbEnums.ScbSystems.CCMS:
                    return GetTypeByProductCategory(ScbEnums.Products.CC, category);
                case ScbEnums.ScbSystems.EBBS:
                    return GetTypeByProductCategory(ScbEnums.Products.SMC, category);
                case ScbEnums.ScbSystems.RLS:
                    return GetTypeByProductCategory(ScbEnums.Products.PL, category);
                default:
                    throw new ArgumentOutOfRangeException("Unknown System : " + systems);
            }
        }

        public static Type GetTypeByProductCategory(ScbEnums.Products product, ScbEnums.Category category)
        {
            var products = DataAccess.GetProductInfo();
            var currentProduct = products.FirstOrDefault(x => x.Product == product);
            if (currentProduct == null)
            {
                throw new InvalidDataException("Missing product information for : " + product);
            }

            if (!currentProduct.HasWriteOff && category == ScbEnums.Category.WriteOff)
            {
                category = ScbEnums.Category.Liner;
            }

            ScbEnums.ClientDataTables tableName;
            switch (category)
            {
                case ScbEnums.Category.Liner:
                    tableName = currentProduct.LinerTable;
                    break;
                case ScbEnums.Category.Payment:
                    tableName = currentProduct.PaymentTable;
                    break;
                case ScbEnums.Category.WriteOff:
                    if (currentProduct.WriteoffTable != null)
                    {
                        tableName = currentProduct.WriteoffTable.Value;
                        break;
                    }
                    throw new InvalidProgramException("NO table exist for the said product-category combination.");
                default:
                    throw new InvalidProgramException("NO table exist for the said product-category combination.");
            }

            return GetClientDataClassTypeByTableName(tableName);
        }

        public static Type GetTypeByProductCategoryForAlloc(ScbEnums.Products product, ScbEnums.Category category)
        {
            var products = DataAccess.GetProductInfo();
            var currentProduct = products.FirstOrDefault(x => x.Product == product);
            if (currentProduct == null)
            {
                throw new InvalidDataException("Missing product information for : " + product);
            }

            if (!currentProduct.HasWriteOff && category == ScbEnums.Category.WriteOff)
            {
                category = ScbEnums.Category.Liner;
            }

            ScbEnums.ClientDataTables tableName;
            switch (category)
            {
                case ScbEnums.Category.Liner:
                    tableName = currentProduct.LinerTable;
                    break;
                case ScbEnums.Category.Payment:
                    tableName = currentProduct.PaymentTable;
                    break;
                case ScbEnums.Category.WriteOff:
                    if (currentProduct.WriteoffTable != null)
                    {
                        tableName = currentProduct.WriteoffTable.Value;
                        break;
                    }
                    throw new InvalidProgramException("NO table exist for the said product-category combination.");
                default:
                    throw new InvalidProgramException("NO table exist for the said product-category combination.");
            }

            return GetClientDataClassTypeByTableNameForAlloc(tableName);
        }

        public static ScbEnums.ScbSystems GetScbSystemByProduct(ScbEnums.Products products)
        {
            switch (products)
            {
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.PL:
                    return ScbEnums.ScbSystems.RLS;
                case ScbEnums.Products.CC:
                    return ScbEnums.ScbSystems.CCMS;
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                case ScbEnums.Products.SME_LAP_OD:
                    return ScbEnums.ScbSystems.EBBS;
                default:
                    throw new ArgumentOutOfRangeException("products");
            }
        }

        public static Type GetPaymentTypeByProduct(ScbEnums.Products products)
        {
            switch (products)
            {
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                case ScbEnums.Products.CC:
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                case ScbEnums.Products.SME_LAP_OD:
                    return typeof(Payment);
                default:
                    throw new ArgumentOutOfRangeException("products");
            }
        }

        public static Type GetClientDataClassTypeByTableName(string tableName)
        {
            var tableEnum = (ScbEnums.ClientDataTables)Enum.Parse(typeof(ScbEnums.ClientDataTables), tableName);
            return GetClientDataClassTypeByTableName(tableEnum);
        }
        public static Type GetAllocDataClassTypeByTableNameForAlloc(ScbEnums.Products products)
        {
            switch (products)
            {
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                case ScbEnums.Products.CC:
                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.SMC:
                case ScbEnums.Products.AUTO_OD:
                    return typeof(Allocations);

                default:
                    throw new ArgumentOutOfRangeException("products");
            }
        }

        private static Type GetClientDataClassTypeByTableName(ScbEnums.ClientDataTables tableName)
        {
            switch (tableName)
            {
                case ScbEnums.ClientDataTables.RLiner:
                    return typeof(RLiner);
                case ScbEnums.ClientDataTables.Payment:
                    return typeof(Payment);
                case ScbEnums.ClientDataTables.RWriteoff:
                    return typeof(RWriteoff);
                case ScbEnums.ClientDataTables.ELiner:
                    return typeof(ELiner);
                
                case ScbEnums.ClientDataTables.EWriteoff:
                    return typeof(EWriteoff);
                case ScbEnums.ClientDataTables.CLiner:
                    return typeof(CLiner);
                case ScbEnums.ClientDataTables.CUnbilled:
                    return typeof(CUnbilled);
                
                case ScbEnums.ClientDataTables.CWriteoff:
                    return typeof(CWriteoff);
                case ScbEnums.ClientDataTables.CacsActivity:
                    return typeof(CacsActivity);
                case ScbEnums.ClientDataTables.Info:
                    return typeof(Info);
                default:
                    throw new ArgumentOutOfRangeException("tableName : " + tableName);
            }
        }
        private static Type GetClientDataClassTypeByTableNameForAlloc(ScbEnums.ClientDataTables tableName)
        {
            switch (tableName)
            {
                case ScbEnums.ClientDataTables.RLiner:
                case ScbEnums.ClientDataTables.ELiner:
                case ScbEnums.ClientDataTables.CLiner:
                case ScbEnums.ClientDataTables.RWriteoff:
                case ScbEnums.ClientDataTables.EWriteoff:
                case ScbEnums.ClientDataTables.CWriteoff:
                    return typeof(Info);
                case ScbEnums.ClientDataTables.Payment:
                    return typeof(Payment);
                case ScbEnums.ClientDataTables.CUnbilled:
                    return typeof(CUnbilled);
                case ScbEnums.ClientDataTables.CacsActivity:
                    return typeof(CacsActivity);
                
                default:
                    throw new ArgumentOutOfRangeException("tableName : " + tableName);
            }
        }

        #endregion

        #region classobject by table-name

        //public static Alloc CreateAllocObject(string className)
        //{
        //    ScbEnums.ClientDataTables tableName;
        //    if (Enum.TryParse(className, out tableName))
        //    {

        //        switch (tableName)
        //        {
        //            case ScbEnums.ClientDataTables.RLiner:
        //            case ScbEnums.ClientDataTables.RWriteoff:
        //            case ScbEnums.ClientDataTables.ELiner:
        //            case ScbEnums.ClientDataTables.EWriteoff:
        //            case ScbEnums.ClientDataTables.CLiner:
        //            case ScbEnums.ClientDataTables.CWriteoff:
        //                return new Alloc();
        //            default:
        //                throw new ArgumentOutOfRangeException("table name for alloc :" + className);
        //        }
        //    }

        //    throw new InvalidProgramException("No such class exist in alloc : " + className);
        //}

        public static Entity GetClientDataClassObjectByTableName(ScbEnums.ClientDataTables tableName)
        {
            var type = GetClientDataClassTypeByTableName(tableName);
            return (UploadableEntity)Activator.CreateInstance(type);
        }

        #endregion

        #region other class types

        public static Type GetType(string className)
        {
            if (string.IsNullOrEmpty(className))
                return null;
            switch (className)
            {
                case "GPincode":
                    return typeof(GPincode);

            }
            return null;
        }

        public static Type GetInfoType(ScbEnums.Products product)
        {
            switch (product)
            {
                case ScbEnums.Products.SME_BIL:
                case ScbEnums.Products.SME_LAP:
                case ScbEnums.Products.SME_ME:
                case ScbEnums.Products.MORT:
                case ScbEnums.Products.AUTO:
                case ScbEnums.Products.PL:
                case ScbEnums.Products.CC:
                case ScbEnums.Products.SME_LAP_OD:
                case ScbEnums.Products.AUTO_OD:
                case ScbEnums.Products.SMC:
                    return typeof(Info);
                default:
                    throw new ArgumentOutOfRangeException("can not fetch info class for product :" + product);
            }
        }

        #endregion
    }
}
