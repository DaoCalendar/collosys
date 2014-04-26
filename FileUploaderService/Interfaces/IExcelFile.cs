using System.Data;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.FileUploadService.Interfaces
{
    interface IExcelFile<TEntity> where TEntity : Entity
    {
        bool PopulateComputedValue(TEntity record, DataRow dr, out string errorDescription);
        TEntity GetByUniqueKey(TEntity record);
        bool CheckBasicField(DataRow dr);
        bool IsRecordValid(TEntity record, out string errorDescription);
        bool PerformUpdates(TEntity record);
    }
}