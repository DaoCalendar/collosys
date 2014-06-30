using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RowCounter;
using ColloSys.FileUploaderService.Utilities;

namespace ColloSys.FileUploaderService.RecordManager
{
    public interface IRecordCreator<TEntity> 
        where TEntity : Entity, IFileUploadable, IUniqueKey
    {
        bool CreateRecord( out TEntity obj);

        IList<TEntity> YesterdayRecords { get; set; }

        MultiKeyEntityList<TEntity> TodayRecordList { get; set; }

        bool EndOfFile();

        void Init(FileScheduler fileScheduler, ICounter counter);
        void InitPreviousDayLiner   (FileScheduler fileScheduler);
    }
}