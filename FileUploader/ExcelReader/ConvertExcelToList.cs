#region reference

using System;
using System.Collections.Generic;
using System.IO;
using ColloSys.FileUploader.Reflection;
using ColloSys.FileUploaderService.ExcelReader;

#endregion

namespace ReflectionExtension.ExcelReader
{
    public class ConvertExcelToList<TU> where TU : class, new()
    {
        #region ctor

        private IExcelReader _iExcelReader;

        public ConvertExcelToList(FileInfo fileInfo)
        {
            var stream = fileInfo.Open(FileMode.Open, FileAccess.Read);
            _iExcelReader = GetInstance(stream);
        }

        public ConvertExcelToList(FileStream fileStream)
        {
            _iExcelReader = GetInstance(fileStream);
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
                foreach (var mapping in mappingList)
                {
                    var value = _iExcelReader.GetValue((uint)mapping.Position);
                    ReflectionHelper.SetValue(mapping.Propname, value, obj);
                }
                objlList.Add(obj);

            }
            return objlList;
        }
        #endregion
    }
}
