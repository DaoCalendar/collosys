using System.Collections.Generic;
using ColloSys.DataLayer.Domain;

namespace ColloSys.FileUploaderService.RecordManager
{
    public interface IRecordCreator<TEntity>
    {
        bool CreateRecord( out TEntity obj);

        IList<TEntity> YesterdayRecords { get; set; }

        bool EndOfFile();

        uint CurrentRow { get; }

        TEntity GetRecordForUpdate();
    }
}