#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.ClientDataBuilder;
using ColloSys.QueryBuilder.GenericBuilder;

#endregion


namespace ColloSys.AllocationService.PincodeEntry
{
    public static class LinerWriteoffPincodes
    {
        private static readonly InfoBuilder InfoBuilder = new InfoBuilder();
        private static readonly GPincodeBuilder GPincodeBuilder=new GPincodeBuilder();
        public static void Init()
        {
            // get pincode list
            var pincodelist = GPincodeBuilder.GetAll().ToList();

            foreach (ScbEnums.Products products in Enum.GetValues(typeof(ScbEnums.Products)))
            {
                if (products == ScbEnums.Products.UNKNOWN)
                    continue;

                var clinerData = InfoBuilder.NonPincodeEntries(products).ToList();
                if (clinerData.Count > 0)
                    AssignPincodes(pincodelist, clinerData);
            }
        }

        private static void AssignPincodes(IList<GPincode> pincodes, List<Info> dataList)
        {
            dataList.ForEach(x => x.GPincode = pincodes
                                                   .Where(y => y.Pincode == x.Pincode)
                                                   .Select(y => y).SingleOrDefault());

            var saveList = dataList.Where(x => x.GPincode != null).ToList();
             
            //set alloc status and Noalloc result
            saveList.ForEach(x =>
                {
                    x.AllocStatus = ColloSysEnums.AllocStatus.None;
                    x.NoAllocResons = ColloSysEnums.NoAllocResons.None;
                    x.AllocEndDate = DateTime.Now;
                });
            InfoBuilder.SaveList(saveList);
        }
    }
}
//var systemOnProduct = Util.GetSystemOnProduct(products);
//switch (systemOnProduct)
//{
//    case ScbEnums.ScbSystems.CCMS:

//        break;
//    case ScbEnums.ScbSystems.EBBS:
//        var elinerData = DbLayer.GetDataLinerWriteOffData<EInfo>(products);
//        if (elinerData.Count > 0)
//            AssignPincodes(pincodelist, elinerData);
//        break;
//    case ScbEnums.ScbSystems.RLS:
//         var linerData = DbLayer.GetDataLinerWriteOffData<RInfo>(products);
//        if (linerData.Count > 0)
//            AssignPincodes(pincodelist, linerData);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}