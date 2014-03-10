#region references

using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.AllocationService.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;

#endregion


namespace ColloSys.AllocationService.CacsToLineWriteoff
{
    public static class MoveDataFromCacs
    {
        public static void Init()
        {
            var fileSchedualrEntries = DataAccess.ReadyToMoveFiles();

            foreach (var fileSchedualrEntry in fileSchedualrEntries)
            {
                var fileschedulerdata = DataAccess.GetDataFromCacs(fileSchedualrEntry);
                foreach (ScbEnums.Products product in Enum.GetValues(typeof(ScbEnums.Products)))
                {
                    if (product == ScbEnums.Products.UNKNOWN)
                        continue;
                    var productData = DataOnProductBase(product, fileschedulerdata);
                    if (!productData.Any())
                        continue;
                    PopulateInfoList<Info>(productData, fileSchedualrEntry);
                }
            }
        }

        private static IList<CacsActivity> DataOnProductBase(ScbEnums.Products product, IEnumerable<CacsActivity> list)
        {
            return (from d in list
                    where d.Products == product
                    select d).ToList();
        }

        private static void PopulateInfoList<TInfo>(IEnumerable<CacsActivity> cacsActivities, FileScheduler fileScheduler)
            where TInfo : Info
        {
            var infodata = DataAccess.GetInfoData(cacsActivities).ToList();
            infodata.ForEach(x =>
            {
                x.AllocEndDate = Util.GetTodayDate();
                x.IsReferred = true;
            });

            fileScheduler.AllocBillDone = true;
            DataAccess.SaveInfoDataWithFileSchedular(infodata, fileScheduler);
        }
    }
}



//private static CacsData MoveToLinerWriteoff<TLiner, TWriteOff>(IEnumerable<CacsActivity> list,FileScheduler fileScheduler)
//    where TLiner : Entity, IFileUploadable, IDelinquentCustomer
//    where TWriteOff : Entity, IFileUploadable, IDelinquentCustomer
//{
//    var cacsData = new CacsData {FileSchedular = fileScheduler};
//    foreach (var cacsActivityRecord in list)
//    {
//        try
//        {
//            var linerRecord = DataAccess.GetAccountData<TLiner>(cacsActivityRecord);
//            var writeoffRecord = DataAccess.GetAccountData<TWriteOff>(cacsActivityRecord);

//            if (linerRecord == null && writeoffRecord == null)
//                throw new NullReferenceException("No basic info found");

//            if (writeoffRecord == null)
//            {
//                SetLinerWriteoffObj(linerRecord, cacsActivityRecord);
//                cacsData.LinerList.Add(linerRecord);
//                //dataForSave.Add(linerRecord);
//                //DataAccess.Save(linerRecord);
//            }

//            if (linerRecord == null)
//            {
//                SetLinerWriteoffObj(writeoffRecord, cacsActivityRecord);
//                cacsData.WriteoffList.Add(writeoffRecord);
//                //dataForSave.Add(writeoffRecord);
//                //DataAccess.Save(writeoffRecord);
//            }

//            if (linerRecord != null && writeoffRecord != null)
//            {
//                if (linerRecord.FileDate > writeoffRecord.FileDate)
//                {
//                    SetLinerWriteoffObj(linerRecord, cacsActivityRecord);
//                    cacsData.LinerList.Add(linerRecord);
//                    //dataForSave.Add(linerRecord);
//                    //DataAccess.Save(linerRecord);
//                }
//                else
//                {
//                    SetLinerWriteoffObj(writeoffRecord, cacsActivityRecord);
//                    cacsData.WriteoffList.Add(writeoffRecord);
//                    //dataForSave.Add(writeoffRecord);
//                    //DataAccess.Save(writeoffRecord);
//                }
//            }


//        }
//        catch (NullReferenceException)
//        {
//            cacsActivityRecord.MissingBasicInfo = true;
//            cacsData.CacsList.Add(cacsActivityRecord);
//            //dataForSave.Add(cacsActivity);
//            //DataAccess.SaveOrUpdate(cacsActivity);
//        }
//    }
//    return cacsData;
//}

//private static void SetLinerWriteoffObj<T>(T obj, IFileUploadable fileUploadable)
//    where T : Entity, IFileUploadable, IDelinquentCustomer
//{
//    obj.AllocStatus = ColloSysEnums.AllocStatus.None;
//    obj.FileScheduler = fileUploadable.FileScheduler;
//    obj.FileDate = fileUploadable.FileDate;
//    obj.FileRowNo = fileUploadable.FileRowNo;
//    obj.Id = Guid.Empty;
//    obj.Version = 0;
//    obj.IsReferred = true;

//    foreach (var property in obj.GetType().GetProperties())
//    {
//        if (!property.PropertyType.IsGenericType)
//        {
//            continue;
//        }

//        var propertyType = property.PropertyType.GetGenericTypeDefinition();
//        if (!typeof(Iesi.Collections.Generic.ISet<>).IsAssignableFrom(propertyType))
//        {
//            continue;
//        }

//        //TODO: fetching multiple interfaces - needs just one - fix that
//        var baseType = property.PropertyType.GetInterfaces()[0].GetGenericArguments()[0];
//        if (baseType.BaseType == typeof (SharedAlloc))
//        {
//            property.SetValue(obj, null);
//        }

//    }
//}



//private static IEnumerable<CacsActivity> DataFromCacsOnFileSchedularId(IEnumerable<FileScheduler> fileSchedulers)
//{
//    var list = new List<CacsActivity>();
//    foreach (var fileScheduler in fileSchedulers)
//    {
//        var templist = DataAccess.GetDataFromCacs(fileScheduler);
//        list.AddRange(templist);
//    }
//    return list;
//}

//var systemOfProduct = Util.GetSystemOnProduct(product);
//switch (systemOfProduct)
//{
//    case ScbEnums.ScbSystems.CCMS:
//        PopulateInfoList<CInfo>(productData, fileSchedualrEntry);
//        break;
//    case ScbEnums.ScbSystems.EBBS:
//        PopulateInfoList<EInfo>(productData, fileSchedualrEntry);
//        break;
//    case ScbEnums.ScbSystems.RLS:
//        PopulateInfoList<RInfo>(productData, fileSchedualrEntry);
//        break;
//    default:
//        throw new ArgumentOutOfRangeException();
//}