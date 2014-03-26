using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;

namespace ColloSys.DataLayer.Mapping
{
    public class FileStatusMap : EntityMap<FileStatus>
    {
        public FileStatusMap()
        {
            Property(x => x.UploadedRows);
            Property(x => x.DuplicateRows);
            Property(x => x.ValidRows);
            Property(x => x.ErrorRows);
            Property(x => x.IgnoredRows);
            Property(x => x.TotalRows);
            Property(x => x.UploadStatus);
            Property(x => x.EntryDateTime);

            ManyToOne(x => x.FileScheduler, map => { map.NotNullable(true); map.Index("IX_FILESTATUS"); });
        }
    }
}