using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.ExcelReader.FileReader
{
    public interface IFileReader<T>
    {
        uint BatchSize { get; }
     void ReadAndSaveBatch(T obj, IExcelReader excelReader, IList<FileMapping> mappings,ICounter counter);

    }

}
