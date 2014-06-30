#region references

using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

#endregion

namespace ColloSys.FileUploaderService.DataLayer
{
   public interface IDbLayer
   {
       FileScheduler GetNextFileForSchedule();

       IList<string> GetValuesFromKey(ColloSysEnums.Activities area, string key);

       List<string> GetAccountNosForDate<TEntity>(DateTime dateTime)  where TEntity : Entity, IUniqueKey;

       IList<TEntity> GetDataForPreviousDay<TEntity>(ColloSysEnums.FileAliasName aliasName,
           DateTime date, uint filecount)
           where TEntity : Entity, IFileUploadable;

       bool SaveOrUpdateData<TEntity>(IList<TEntity> data)
           where TEntity : Entity;

       TEntity GetRecordForUpdate<TEntity>(string accountNo)
           where TEntity : Entity, IDelinquentCustomer;

       IList<TEntity> GetPreviousRecords<TEntity>(ScbEnums.Products products)
           where TEntity : Entity, IDelinquentCustomer;
       IList<TEntity> GetDataForDate<TEntity>(DateTime fileDate)
           where TEntity : Entity, IFileUploadable;
   }
}
