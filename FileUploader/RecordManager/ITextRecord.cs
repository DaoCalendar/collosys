namespace ColloSys.FileUploaderService.RecordManager
{
    public interface ITextRecord<TEntity> : IRecordCreator<TEntity> where TEntity : class, new()
    {
        
    }
}