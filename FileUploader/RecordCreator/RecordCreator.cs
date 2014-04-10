using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.ExcelReaders.RecordSetter;
using ColloSys.FileUploader.Reflection;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.RecordCreator
{
    public class RecordCreator<TEntity> : IRecord<TEntity> where TEntity : class , new()
    {
        #region ctor
        private readonly IAliasRecordCreator<TEntity> _recordCreator;
        public RecordCreator(IAliasRecordCreator<TEntity> recordCreator)
        {
            _recordCreator = recordCreator;
        }
        #endregion

        #region IRecord
        public bool ExcelMapper(TEntity obj, IExcelReader reader, IEnumerable<FileMapping> mappings, ICounter counter)
        {
            foreach (var info in mappings)
            {
                try
                {
                    var data = reader.GetValue(info.Position);
                    ReflectionHelper.SetValue(info.ActualColumn, data, obj);
                }
                catch
                {
                    counter.IncrementErrorRecords();
                    counter.IncrementTotalRecords();
                    //throw new Exception(string.Format("Column {0} is Not Getting Proper Value .", info.ActualColumn), e);
                    return false;
                }
            }
            return true;
        }

        public bool DefaultMapper(TEntity obj, IEnumerable<FileMapping> mapings, ICounter counter)
        {
            foreach (var mapping in mapings)
            {
                try
                {
                    ReflectionHelper.SetValue(mapping.ActualColumn, mapping.DefaultValue, obj);
                }
                catch
                {
                    counter.IncrementErrorRecords();
                    counter.IncrementTotalRecords();
                    return false;
                }

            }
            return true;
        }

        public bool CreateRecord(TEntity obj, IExcelReader reader, IList<FileMapping> mappingss, ICounter counter)
        {
            bool excelstatus = false, defaultMap = false, computedMap = false;

            var excelType = GetMappings(ColloSysEnums.FileMappingValueType.ExcelValue, mappingss);
            if (excelType.Any())
            {
                if (!_recordCreator.CheckBasicField(reader, excelType, counter))
                    return false;

                excelstatus = ExcelMapper(obj, reader, excelType, counter);
            }
            if (!excelstatus) return false;
            var defaultType = GetMappings(ColloSysEnums.FileMappingValueType.DefaultValue, mappingss);
            var typeDefault = defaultType as FileMapping[] ?? defaultType.ToArray();
            if (typeDefault.Any())
            {
                defaultMap = DefaultMapper(obj, typeDefault, counter);
            }

            var computedType = GetMappings(ColloSysEnums.FileMappingValueType.ComputedValue, mappingss);
            var typeComputed = computedType as FileMapping[] ?? computedType.ToArray();
            if (typeComputed.Any())
            {
                computedMap = _recordCreator.ComputedSetter(obj, reader, counter);
            }
            if (!defaultMap && !computedMap) return false;

            counter.IncrementInsertRecords();
            counter.IncrementValidRecords();
            counter.IncrementTotalRecords();
            return true;
        }

        public IList<FileMapping> GetMappings(ColloSysEnums.FileMappingValueType eNumType, IEnumerable<FileMapping> mappingss)
        {
            return mappingss.Where(x => x.ValueType == eNumType).ToList();
        }
        #endregion
    }
}
