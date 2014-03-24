#region reference

using System;
using System.Collections.Generic;
using System.IO;
using FileUploader.Reflection;

#endregion

namespace FileUploader.ExcelReader
{
    public class ConvertExcelToList<TU> where TU : class, new()
    {
        #region ctor

        private IExcelReader _iExcelReader;

        public ConvertExcelToList(FileStream fileInfo)
        {
            _iExcelReader = GetInstance(fileInfo);
        }

        public ConvertExcelToList(IExcelReader reader)
        {
            _iExcelReader = reader;
        }

        private IExcelReader GetInstance(FileStream fileInfo)
        {
            var file = Path.GetExtension(fileInfo.Name);
            switch (file)
            {
                case ".xlsx":
                    return _iExcelReader = new EpPlusExcelReader(fileInfo);
                case ".xls":
                    return _iExcelReader = new NpOiExcelReader(fileInfo);
                default:
                    throw new Exception("Invalid File");
            }
        }

        #endregion

        #region setvalue
        public IList<TU> GetList(IList<MappingInfo> mappingList)
        {
            var objlList = new List<TU>();

            for (var i = 0; i < _iExcelReader.TotalRows; i++)
            {
                var obj = (TU)Activator.CreateInstance(typeof(TU));
                _iExcelReader.NextRow();
                foreach (var mapping in mappingList)
                {
                    var value = _iExcelReader.GetValue((uint)mapping.Position);
                    ReflectionHelper.SetValue(mapping.Propname, value, obj);
                }
                objlList.Add(obj);
               
            }
            return objlList;
        }

        public object SetValue(string propName, int row, int col)
        {
            var obj = Activator.CreateInstance(typeof(TU));
            string value = _iExcelReader.GetValue((uint)col, (uint)row);
            ReflectionHelper.SetValue(propName, value, obj);
            return obj;
        }
        #endregion
    }
}
