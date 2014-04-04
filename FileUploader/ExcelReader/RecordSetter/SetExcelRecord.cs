using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.Reflection;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.ExcelReader.RecordSetter
{
    public class SetExcelRecord<T> : IExcelRecord<T> where T : class , new()
    {
        private readonly ICounter _counter;

        public SetExcelRecord()
        {
            _counter = new ExcelRecordCounter();
        }

        public bool UniqExcelMapper(T obj, IExcelReader reader, IList<FileMapping> mapings)
        {
                var uniqColumnMapping = from d in mapings where (d.ActualColumn == "AccountNo") select d;
                string data = reader.GetValue(uniqColumnMapping.FirstOrDefault().Position);
                if (data != "" && IsDigit(data))
                {
                    ReflectionHelper.SetValue(mapings.FirstOrDefault().ActualColumn, data, obj);
                    return true;
                }
                _counter.IncrementIgnoreRecord();
                return false;
        }

        private bool IsDigit(string str)
        {
            return str.All(s => char.IsDigit(s));
        }

        public bool ExcelMapper(T obj, IExcelReader reader, IList<FileMapping> mappings)
        {
           
            if (UniqExcelMapper(obj, reader, mappings))
            {
                foreach (var info in mappings)
                {
                    var data = reader.GetValue(info.Position);
                    ReflectionHelper.SetValue(info.ActualColumn, data, obj);
                }
                return true;
            }
            return false;
        }

        public bool DefaultMapper(T obj, IList<FileMapping> mapings)
        {
            foreach (var mapping in mapings)
            {
                ReflectionHelper.SetValue(mapping.ActualColumn, mapping.DefaultValue, obj);
            }
            return true;
        }



        public bool ComputedSetter(T obj, IExcelReader reader)
        {
            var value = reader.GetValue(4) + reader.GetValue(5);
            ReflectionHelper.SetValue("TransAmount", value, obj);
            return true;
        }

        public bool ComputedSetter(T obj, object yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            var data = reader.GetValue(1) + 5;
            ReflectionHelper.SetValue("Default", data, obj);
            return true;
        }

        public bool CreateRecord(T obj, IExcelReader reader, IList<FileMapping> mappingss)
        {
            bool excelstatus = false, defaultStatus=false, coumputedStatus=false;
            var excelType = (from m in mappingss
                             where (m.ValueType == ColloSysEnums.FileMappingValueType.ExcelValue)
                             select m).ToList();
            if (excelType.Any())
            {
                excelstatus = ExcelMapper(obj, reader, excelType);
            }
            if (excelstatus)
            {
                var defaultType = from m in mappingss
                                  where (m.ValueType == ColloSysEnums.FileMappingValueType.DefaultValue)
                                  select (m);
                var typeDefault = defaultType as FileMapping[] ?? defaultType.ToArray();
                if (typeDefault.Any())
                {
                    defaultStatus = DefaultMapper(obj, typeDefault);
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
                _counter.IncrementInsertRecords();
                _counter.IncrementValidRecords();

                return true;
            }
            return false;

        }

        public ulong TotalCount { get { return _counter.TotalRecords; } }//remove this


    }
}
