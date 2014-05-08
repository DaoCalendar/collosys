using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.FileReader
{
    public interface IFileReader<T>
    {
        void ReadAndSaveBatch();
        void Save();
    }

}
