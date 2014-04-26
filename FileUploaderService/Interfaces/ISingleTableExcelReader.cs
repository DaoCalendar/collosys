using System.Data;
using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.FileUploadService.Interfaces
{
    public interface IExcelFile<TEntity> where TEntity : Entity
    {
        bool PopulateComputedValue(TEntity record, out string errorDescription);
        bool CheckBasicField(DataRow dr);
        bool IsRecordValid(TEntity record, out string errorDescription);
        TEntity GetByUniqueKey(TEntity record);
        bool PerformUpdates(TEntity record);
        //void AddRecordInListByUniqueKey(TEntity record, List<TEntity> recordList);
    }
}