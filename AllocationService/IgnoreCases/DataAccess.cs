#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.AllocationBuilder;
using ColloSys.QueryBuilder.ClientDataBuilder;

#endregion


namespace ColloSys.AllocationService.IgnoreCases
{
    internal static class DataAccess
    {
        private static readonly InfoBuilder InfoBuilder=new InfoBuilder();
        private static readonly AllocGenericCalls AllocGenericCalls=new AllocGenericCalls();

        public static IEnumerable<Info> GetInfoData(ScbEnums.Products products)
        {
            return InfoBuilder.IgnoreAllocated(products);
        }

        public static TLinerWriteOff CheckInInfo<TLinerWriteOff>(TLinerWriteOff linerWriteOff)
            //where TInfo : SharedInfo
            where TLinerWriteOff : Entity, IDelinquentCustomer
        {
            return AllocGenericCalls.CheckInInfo(linerWriteOff);
        }
    }
}
