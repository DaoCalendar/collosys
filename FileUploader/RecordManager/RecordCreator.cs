#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.Reflection;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.RecordManager
{
    public abstract class RecordCreator<TEntity> : IRecord<TEntity> where TEntity : class , new()
    {
        #region ctor
        private IExcelReader _reader;
        private ICounter _counter;
        protected FileScheduler FileScheduler;

        public void Init(FileScheduler fileScheduler, ICounter counter, IExcelReader reader)
        {
            FileScheduler = fileScheduler;
            _reader = reader;
            _counter = counter;
        }
        #endregion

        #region IRecord
        public bool ExcelMapper(TEntity obj, IList<FileMapping> mappings)
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

        public bool DefaultMapper(TEntity obj, IList<FileMapping> mapings)
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

        public bool CreateRecord(TEntity obj, IList<FileMapping> mappingss,ICounter counter)
        {
            bool excelstatus = false, defaultMap = false, computedMap = true;

            var excelType = GetMappings(ColloSysEnums.FileMappingValueType.ExcelValue, mappingss);
            if (excelType.Any())
            {
                if (!CheckBasicField(_reader, _counter))
                    return false;

                excelstatus = ExcelMapper(obj, excelType);
            }
            if (!excelstatus || !IsRecordValid(obj, _counter)) return false;

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
                computedMap = ComputedSetter(obj, _reader, _counter);
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

        #region abstract
        protected abstract bool ComputedSetter(TEntity entity, IExcelReader reader, ICounter counter);

        protected abstract bool IsRecordValid(TEntity entity, ICounter counter);

        protected abstract bool CheckBasicField(IExcelReader reader, ICounter counter);
        #endregion
    }
}
