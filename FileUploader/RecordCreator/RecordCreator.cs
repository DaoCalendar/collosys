#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.Reflection;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploader.RecordCreator
{
    public class RecordCreator<TEntity> : IRecord<TEntity> where TEntity : class , new()
    {
        #region ctor
        private readonly IAliasRecordCreator<TEntity> _recordCreator;
        private readonly IExcelReader _reader;
        private readonly ICounter _counter;
        public RecordCreator(IAliasRecordCreator<TEntity> recordCreator, IExcelReader reader, ICounter counter)
        {
            _recordCreator = recordCreator;
            _reader = reader;
            _counter = counter;
        }
        #endregion

        #region IRecord
        public bool ExcelMapper(TEntity obj, IEnumerable<FileMapping> mappings)
        {
            foreach (var info in mappings)
            {
                try
                {
                    var data = _reader.GetValue(info.Position);
                    ReflectionHelper.SetValue(info.ActualColumn, data, obj);
                }
                catch
                {
                    _counter.IncrementErrorRecords();
                    _counter.IncrementTotalRecords();
                   return false;
                }
            }
            return true;
        }

        public bool DefaultMapper(TEntity obj, IEnumerable<FileMapping> mapings)
        {
            foreach (var mapping in mapings)
            {
                try
                {
                    ReflectionHelper.SetValue(mapping.ActualColumn, mapping.DefaultValue, obj);
                }
                catch
                {
                    _counter.IncrementErrorRecords();
                    _counter.IncrementTotalRecords();
                    return false;
                }

            }
            return true;
        }

        public bool CreateRecord(TEntity obj, IList<FileMapping> mappingss)
        {
            bool excelstatus = false, defaultMap = false, computedMap = true;

            var excelType = GetMappings(ColloSysEnums.FileMappingValueType.ExcelValue, mappingss);
            if (excelType.Any())
            {
                if (!_recordCreator.CheckBasicField(_reader, excelType, _counter))
                    return false;

                excelstatus = ExcelMapper(obj, excelType);
            }
            if (!excelstatus) return false;
            var defaultType = GetMappings(ColloSysEnums.FileMappingValueType.DefaultValue, mappingss);
            var typeDefault = defaultType as FileMapping[] ?? defaultType.ToArray();
            if (typeDefault.Any())
            {
                defaultMap = DefaultMapper(obj, typeDefault);
            }

            var computedType = GetMappings(ColloSysEnums.FileMappingValueType.ComputedValue, mappingss);
            var typeComputed = computedType as FileMapping[] ?? computedType.ToArray();
            if (typeComputed.Any())
            {
                computedMap = _recordCreator.ComputedSetter(obj, _reader, _counter);
            }
            if (!defaultMap || !computedMap) return false;

            _counter.IncrementInsertRecords();
            _counter.IncrementValidRecords();
            _counter.IncrementTotalRecords();
            return true;
        }

        public IList<FileMapping> GetMappings(ColloSysEnums.FileMappingValueType eNumType, IEnumerable<FileMapping> mappingss)
        {
            return mappingss.Where(x => x.ValueType == eNumType).ToList();
        }
        #endregion
    }
}
