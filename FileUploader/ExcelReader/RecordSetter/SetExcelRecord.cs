using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.Reflection;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.ExcelReader.RecordSetter
{
    public class SetExcelRecord<TEntity> : IExcelRecord<TEntity> where TEntity : class , new()
    {
        public bool UniqExcelMapper(TEntity obj, IExcelReader reader, IList<FileMapping> mapings,ICounter counter)
        {
            var uniqColumnMapping = from d in mapings where (d.ActualColumn == "AccountNo") select d;
            string data = reader.GetValue(uniqColumnMapping.FirstOrDefault().Position);
            if (data != "" && IsDigit(data))
            {
                return true;
            }
            counter.IncrementIgnoreRecord();
            return false;
        }

        private bool IsDigit(string str)
        {
            return str.All(char.IsDigit);
        }

        public bool ExcelMapper(TEntity obj, IExcelReader reader, IList<FileMapping> mappings,ICounter counter)
        {
            if (!UniqExcelMapper(obj, reader, mappings, counter)) 
                return false;

            foreach (var info in mappings)
            {
                try
                {
                    var data = reader.GetValue(info.Position);
                    ReflectionHelper.SetValue(info.ActualColumn, data, obj);
                }
                catch (Exception e)
                {
                    counter.IncrementErrorRecords();
                    throw new Exception(string.Format("Column {0} is Not Getting Proper Value .", info.ActualColumn), e);
                }

            }
            return true;
        }

        public bool DefaultMapper(TEntity obj, IEnumerable<FileMapping> mapings,ICounter counter)
        {
            foreach (var mapping in mapings)
            {
                try
                {
                    ReflectionHelper.SetValue(mapping.ActualColumn, mapping.DefaultValue, obj);
                }
                catch (Exception exception)
                {
                    counter.IncrementErrorRecords();
                    throw new Exception(string.Format("Column {0} is Not Getting Proper Value .", mapping.ActualColumn), exception);
                }

            }
            return true;
        }



        public bool ComputedSetter(TEntity obj, IExcelReader reader)
        {
            var value = reader.GetValue(4) + reader.GetValue(5);
            ReflectionHelper.SetValue("TransAmount", value, obj);
            return true;
        }

        public bool ComputedSetter(TEntity obj, object yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            var data = reader.GetValue(1) + 5;
            ReflectionHelper.SetValue("Default", data, obj);
            return true;
        }

        public bool CreateRecord(TEntity obj, IExcelReader reader, IList<FileMapping> mappingss,ICounter counter)
        {
            bool excelstatus = false, defaultStatus = false, coumputedStatus = false;
            var excelType = (from m in mappingss
                             where (m.ValueType == ColloSysEnums.FileMappingValueType.ExcelValue)
                             select m).ToList();
            if (excelType.Any())
            {
                excelstatus = ExcelMapper(obj, reader, excelType,counter);
            }
            if (excelstatus)
            {
                var defaultType = from m in mappingss
                                  where (m.ValueType == ColloSysEnums.FileMappingValueType.DefaultValue)
                                  select (m);
                var typeDefault = defaultType as FileMapping[] ?? defaultType.ToArray();
                if (typeDefault.Any())
                {
                    defaultStatus = DefaultMapper(obj, typeDefault,counter);
                }

                var computedType = from m in mappingss
                                   where (m.ValueType == ColloSysEnums.FileMappingValueType.ComputedValue)
                                   select (m);
                var typeComputed = computedType as FileMapping[] ?? computedType.ToArray();
                if (typeComputed.Any())
                {
                    coumputedStatus = ComputedSetter(obj, reader);
                }

                if (!excelstatus && !coumputedStatus && !defaultStatus) return false;
                counter.IncrementInsertRecords();
                counter.IncrementValidRecords();

                return true;
            }
            return false;
        }
    }
}
