using System;
using System.Collections.Generic;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.Reflection;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.ExcelReader.RecordSetter
{
    public class CreateRecords<TEntity> : IExcelRecord<TEntity> where TEntity : class , new()
    {
        public bool UniqExcelMapper(TEntity obj, IExcelReader reader, IList<FileMapping> mapings, ICounter counter)
        {
            var uniqColumnMapping = from d in mapings where (d.ActualColumn == "AccountNo") select d;
            var mapping = uniqColumnMapping.FirstOrDefault();
            if (mapping != null)
            {
                string data = reader.GetValue(mapping.Position);
                if (data != "" && SharedUtility.IsDigit(data))
                {
                    return true;
                }
            }
            counter.IncrementIgnoreRecord();
            counter.IncrementTotalRecords();
            return false;
        }

        public bool ExcelMapper(TEntity obj, IExcelReader reader, IList<FileMapping> mappings, ICounter counter)
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



        public bool ComputedSetter(TEntity obj, IExcelReader reader, ICounter counter)
        {
            try
            {
                var value = reader.GetValue(4) + reader.GetValue(5);
                ReflectionHelper.SetValue("TransAmount", value, obj);
                return true;
            }
            catch (Exception exception)
            {

                counter.IncrementErrorRecords();
                counter.IncrementTotalRecords();
                throw new Exception(string.Format("Computed Column is Not Getting Proper Value"), exception);
         
            }
           
        }

        public bool ComputedSetter(TEntity obj, object yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            var data = reader.GetValue(1) + 5;
            ReflectionHelper.SetValue("Default", data, obj);
            return true;
        }

        public bool CreateRecord(TEntity obj, IExcelReader reader, IList<FileMapping> mappingss, ICounter counter)
        {
            bool excelstatus = false,defaultMap=false,computedMap=false;

            var excelType = getMappings(ColloSysEnums.FileMappingValueType.ExcelValue, mappingss);
            if (excelType.Any())
            {
                if (!UniqExcelMapper(obj, reader, excelType, counter))
                    return false;

                excelstatus = ExcelMapper(obj, reader, excelType, counter);
            }
            if (!excelstatus) return false;
            var defaultType = getMappings(ColloSysEnums.FileMappingValueType.DefaultValue, mappingss);
            var typeDefault = defaultType as FileMapping[] ?? defaultType.ToArray();
            if (typeDefault.Any())
            {
               defaultMap=  DefaultMapper(obj, typeDefault, counter);
            }

            var computedType = getMappings(ColloSysEnums.FileMappingValueType.ComputedValue, mappingss);
            var typeComputed = computedType as FileMapping[] ?? computedType.ToArray();
            if (typeComputed.Any())
            {
                computedMap= ComputedSetter(obj, reader,counter);
            }
            if (!defaultMap && !computedMap) return false;
           
            counter.IncrementInsertRecords();
            counter.IncrementValidRecords();
            counter.IncrementTotalRecords();
            return true;
        }

        private IList<FileMapping> getMappings(ColloSysEnums.FileMappingValueType eNumType, IEnumerable<FileMapping> mappingss)
        {
            return (from m in mappingss
                    where (m.ValueType == eNumType)
                    select m).ToList();
        }
    }
}
