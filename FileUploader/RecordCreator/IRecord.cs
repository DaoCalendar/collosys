using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.RecordCreator
{
    public interface IRecord<in TEntity> where TEntity : class , new()
    {
        IList<FileMapping> GetMappings(ColloSysEnums.FileMappingValueType eNumType, IEnumerable<FileMapping> mappingss);

        bool ExcelMapper(TEntity obj, IExcelReader reader, IEnumerable<FileMapping> mapings, ICounter counter);

        bool DefaultMapper(TEntity obj, IEnumerable<FileMapping> mapings, ICounter counter);

        bool CreateRecord(TEntity obj, IExcelReader reader, IList<FileMapping> mapings, ICounter counter);
    }
}
