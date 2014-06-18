#region references

using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.Reflection;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploaderService.DbLayer;
using NLog;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.RecordManager
{
    public abstract class RecordCreator<TEntity> : IRecord<TEntity> where TEntity : class , new()
    {
        #region ctor
        protected IExcelReader Reader;
        protected ICounter Counter;
        protected FileScheduler FileScheduler;
        protected IDbLayer _DbLayer;
        protected readonly Logger _log = LogManager.GetCurrentClassLogger();
       

        public void Init(FileScheduler fileScheduler, ICounter counter, IExcelReader reader)
        {
            FileScheduler = fileScheduler;
            Reader = reader;
            Counter = counter;
            _DbLayer=new DbLayer.DbLayer();
        }

        public IList<TEntity> PreviousDayLiner { get; set; }

        public bool HasMultiDayComputation { get; set; }

        #endregion

        #region IRecord
        public bool ExcelMapper(TEntity obj, IList<FileMapping> mappings)
        {
            foreach (var info in mappings)
            {
                try
                {
                    var data = Reader.GetValue(info.Position);
                    ReflectionHelper.SetValue(info.ActualColumn, data, obj);
                }
                catch
                {
                    Counter.IncrementErrorRecords();
                    Counter.IncrementTotalRecords();
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
                    Counter.IncrementErrorRecords();
                    Counter.IncrementTotalRecords();
                    return false;
                }

            }
            return true;
        }

        public bool CreateRecord(IList<FileMapping> mappingss,out TEntity obj)
        {
            bool excelstatus = false, defaultMap = false, computedMap = true;

            obj = GetRecordForUpdate();

            var excelType = GetMappings(ColloSysEnums.FileMappingValueType.ExcelValue, mappingss);
            if (excelType.Any())
            {
                if (!CheckBasicField())
                    return false;

                excelstatus = ExcelMapper(obj, excelType);
            }
            if (!excelstatus || !IsRecordValid(obj)) return false;

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
                computedMap = ComputedSetter(obj);
            }
            if (HasMultiDayComputation)
            {
                var preEntity = GetPreviousDayEntity(obj);
                computedMap = ComputedSetter(obj, preEntity);
            }
            if (!defaultMap || !computedMap) return false;

            Counter.IncrementInsertRecords();
            Counter.IncrementValidRecords();
            Counter.IncrementTotalRecords();
            return true;
        }

        public IList<FileMapping> GetMappings(ColloSysEnums.FileMappingValueType eNumType, IEnumerable<FileMapping> mappingss)
        {
            return mappingss.Where(x => x.ValueType == eNumType).ToList();
        }
        #endregion

        #region abstract
        public abstract bool ComputedSetter(TEntity entity);
        public abstract bool ComputedSetter(TEntity entity, TEntity preEntity);
       
        public abstract bool IsRecordValid(TEntity entity);

        public abstract bool CheckBasicField();

        public abstract TEntity GetRecordForUpdate();

        public abstract TEntity GetPreviousDayEntity(TEntity entity);

        #endregion
    }
}
