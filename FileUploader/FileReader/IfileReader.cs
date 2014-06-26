using System.Collections.Generic;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;
using ColloSys.FileUploaderService.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.FileReader
{
    public interface IFileReader<T> where T:class,new()
    {
        ICounter Counter { get; }
        //IExcelReader ExcelReader { get; }
        IRecordCreator<T> RecordCreatorObj { get; }
        FileScheduler FileScheduler { get;  }
    
        void ReadAndSaveBatch();
        void ProcessFile();
        IList<T> GetNextBatch();
    }

}
