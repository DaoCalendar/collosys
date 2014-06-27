using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RowCounter;

namespace ColloSys.FileUploaderService.RecordManager
{
    public interface ITextRecord<TEntity> : IRecordCreator<TEntity> 
        where TEntity : Entity, IFileUploadable, IUniqueKey, new()
    {
        void Init(FileScheduler fileScheduler, ICounter counter);

        void InitPreviousDayLiner(FileScheduler fileScheduler);
    }
}