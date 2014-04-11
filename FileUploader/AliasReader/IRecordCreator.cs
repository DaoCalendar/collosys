using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
    public interface IAliasRecordCreator<in TEntity> where TEntity : class, new()
    {
        bool ComputedSetter(TEntity obj, IExcelReader reader, ICounter counter);

        bool ComputedSetter(TEntity obj, object yobj, IExcelReader reader,
            IEnumerable<FileMapping> mapplings);

        bool CheckBasicField(IExcelReader reader, IEnumerable<FileMapping> mapings, ICounter counter);

        bool IsRecordValid(TEntity record);
    }
}