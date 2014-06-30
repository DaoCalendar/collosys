#region references

using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.Reflection;
using ColloSys.FileUploader.Utilities;
using ColloSys.FileUploaderService.DbLayer;
using ColloSys.FileUploaderService.ExcelReader;
using ColloSys.FileUploaderService.RowCounter;
using ColloSys.FileUploaderService.Utilities;
using NLog;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.RecordManager
{
    public abstract class RecordCreator<TEntity> : IExcelRecord<TEntity> 
        where TEntity : Entity, IFileUploadable, IUniqueKey, new()
    {
        #region ctor
        protected IExcelReader Reader;
        protected ICounter Counter;
        protected FileScheduler FileScheduler;
        protected readonly IDbLayer DbLayer = new DbLayer.DbLayer();
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected RecordCreator()
        {
            TodayRecordList = new MultiKeyEntityList<TEntity>(); 
        } 

        public void Init(FileScheduler fileScheduler, ICounter counter)
        {
            FileScheduler = fileScheduler;
            Reader = SharedUtility.GetInstance(new FileInfo(FileScheduler.FileDirectory + @"\" + FileScheduler.FileName),counter);
            Counter = counter;
        }

        public MultiKeyEntityList<TEntity> TodayRecordList { get; set; }

        public bool EndOfFile()
        {
            return Reader.EndOfFile();
        }

        public uint CurrentRow
        {
            get { return Reader.CurrentRow; }
        }

        public IList<TEntity> YesterdayRecords { get; set; }

        public bool HasMultiDayComputation { get; set; }

        public void InitPreviousDayLiner( FileScheduler fileScheduler)
        {
            if (HasMultiDayComputation)
                YesterdayRecords = DbLayer.GetDataForPreviousDay<TEntity>(fileScheduler.FileDetail.AliasName,
                   fileScheduler.FileDate, fileScheduler.FileDetail.FileCount);
        }

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

        public bool CreateRecord( out TEntity obj)
        {
            bool excelstatus = false, defaultMap = true, computedMap = true;

            obj = GetRecordForUpdate();

            var excelType = GetMappings(ColloSysEnums.FileMappingValueType.ExcelValue, FileScheduler.FileDetail.FileMappings);
            if (excelType.Any())
            {
                if (!CheckBasicField())
                {
                    Counter.IncrementIgnoreRecord();
                    return false;
                }
             

                excelstatus = ExcelMapper(obj, excelType);
            }
            if (!excelstatus || !IsRecordValid(obj)) return false;

            var defaultType = GetMappings(ColloSysEnums.FileMappingValueType.DefaultValue, FileScheduler.FileDetail.FileMappings);
            var typeDefault = defaultType as FileMapping[] ?? defaultType.ToArray();
            if (typeDefault.Any())
            {
                defaultMap = DefaultMapper(obj, typeDefault);
            }

            var computedType = GetMappings(ColloSysEnums.FileMappingValueType.ComputedValue, FileScheduler.FileDetail.FileMappings);
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
            Reader.NextRow();
            
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
