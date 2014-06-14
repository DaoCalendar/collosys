#region references

using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.RecordManager
{
    public interface IRecord<in TEntity> where TEntity : class , new()
    {
        IList<FileMapping> GetMappings(ColloSysEnums.FileMappingValueType eNumType, IEnumerable<FileMapping> mappingss);

        bool ExcelMapper(TEntity obj, IList<FileMapping> mapings);

        bool DefaultMapper(TEntity obj, IList<FileMapping> mapings);

        bool CreateRecord(TEntity obj, IList<FileMapping> mapings,ICounter counter);

        void Init(FileScheduler fileScheduler, ICounter counter, IExcelReader reader);
    }
}
