 using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
    public interface IAliasRecordCreator<in TEntity> where TEntity : class, new()
    {
        FileScheduler FileScheduler { get; }

        bool ComputedSetter(TEntity obj, IExcelReader reader, ICounter counter);

        bool ComputedSetter(TEntity obj, TEntity yobj, IExcelReader reader,
            IEnumerable<FileMapping> mapplings);

        bool CheckBasicField(IExcelReader reader, ICounter counter);

        bool IsRecordValid(TEntity record,ICounter counter);
    }
}