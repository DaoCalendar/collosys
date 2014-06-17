#region references

using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.RecordManager
{
    public interface IRecord<TEntity> where TEntity : class , new()
    {
        IList<FileMapping> GetMappings(ColloSysEnums.FileMappingValueType eNumType, IEnumerable<FileMapping> mappingss);

        bool ExcelMapper(TEntity obj, IList<FileMapping> mapings);

        bool DefaultMapper(TEntity obj, IList<FileMapping> mapings);

        TEntity GetRecordForUpdate();

        bool CreateRecord(IList<FileMapping> mapings,out TEntity obj);

        bool ComputedSetter(TEntity entity);
        bool ComputedSetter(TEntity entity, TEntity preEntity);

        bool IsRecordValid(TEntity entity);

        bool CheckBasicField();

        void Init(FileScheduler fileScheduler, ICounter counter, IExcelReader reader);

        IList<TEntity> PreviousDayLiner { get; set; }

        bool HasMultiDayComputation { get; set; }


    }
}
