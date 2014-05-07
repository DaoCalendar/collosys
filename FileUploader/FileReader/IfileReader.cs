using System.Collections.Generic;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.FileReader
{
    public interface IFileReader<T>
    {
        IList<T> List { get; }
        void ReadAndSaveBatch();

        void Save(ICounter counter);
    }

}
