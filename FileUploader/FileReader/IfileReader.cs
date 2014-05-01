using System.Collections.Generic;

namespace ColloSys.FileUploader.FileReader
{
    public interface IFileReader<T>
    {
        IList<T> List { get; }
        void ReadAndSaveBatch();
    }

}
