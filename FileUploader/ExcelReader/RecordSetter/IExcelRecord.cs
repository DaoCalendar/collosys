using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.ExcelReader.RecordSetter
{
    public interface IExcelRecord<TEntity> where TEntity : class , new()
    {
        bool ExcelMapper(TEntity obj, IExcelReader reader, IList<FileMapping> mapings, ICounter counter);

        bool DefaultMapper(TEntity obj, IEnumerable<FileMapping> mapings, ICounter counter);

        bool ComputedSetter(TEntity obj, IExcelReader reader);

        bool ComputedSetter(TEntity obj, object yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings);

        bool CreateRecord(TEntity obj, IExcelReader reader, IList<FileMapping> mapings, ICounter counter);

        bool UniqExcelMapper(TEntity obj, IExcelReader reader, IList<FileMapping> mapings, ICounter counter);
    }
}
