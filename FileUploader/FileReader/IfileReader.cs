using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploaderService.RecordManager;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.FileReader
{
    public interface IFileReader<T> where T:class,new()
    {
        ICounter Counter { get; set; }
        IExcelReader ExcelReader { get; }
        IRecord<T> ObjRecord { get; }
        FileScheduler _fs { get;  }
    
        void ReadAndSaveBatch();
        void ProcessFile();
        IList<T> GetNextBatch();

        //void PreProcessing();
        bool PostProcessing();
    }

}
