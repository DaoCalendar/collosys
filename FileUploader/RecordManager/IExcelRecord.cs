#region references

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RowCounter;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.RecordManager
{
    public interface IExcelRecord<TEntity> : IRecordCreator<TEntity> where TEntity :  Entity, IFileUploadable, IUniqueKey, new()
    {
        IList<FileMapping> GetMappings(ColloSysEnums.FileMappingValueType eNumType, IEnumerable<FileMapping> mappingss);

        bool ExcelMapper(TEntity obj, IList<FileMapping> mapings);

        bool DefaultMapper(TEntity obj, IList<FileMapping> mapings);

        bool ComputedSetter(TEntity entity);
        bool ComputedSetter(TEntity entity, TEntity preEntity);

        bool IsRecordValid(TEntity entity);

        bool CheckBasicField();

        bool HasMultiDayComputation { get; set; }
    }
}
