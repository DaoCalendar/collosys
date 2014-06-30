using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Components;

namespace ColloSys.FileUploaderService.RecordManager
{
    public interface ITextRecord<TEntity> : IRecordCreator<TEntity> 
        where TEntity : Entity, IFileUploadable, IUniqueKey, new()
    {
       
    }
}