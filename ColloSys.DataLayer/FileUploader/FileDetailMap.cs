using ColloSys.DataLayer.BaseEntity;

namespace ColloSys.DataLayer.FileUploader
{
    public class FileDetailMap : EntityMap<FileDetail>
    {
        public FileDetailMap()
        {
            Property(x => x.AliasName, map =>{map.UniqueKey("UK_FILE_DETAILS");map.Index("IX_FILE_DETAILS");});
            Property(x => x.AliasDescription);
            Property(x => x.FileName);
            Property(x => x.FileCount);
            Property(x => x.DependsOnAlias);
            Property(x => x.FileReaderType);
            Property(x => x.DateFormat);
            Property(x => x.FileServer);
            Property(x => x.FileDirectory);
            Property(x => x.FileType);
            Property(x => x.SheetName);
            Property(x => x.Frequency);
            Property(x => x.SkipLine);
            Property(x => x.ActualTable);
            Property(x => x.TempTable);
            Property(x => x.ErrorTable);
            Property(x => x.EmailId);
            Property(x => x.Description, map => map.NotNullable(false));
            Property(x => x.UsedFor);
            Property(x => x.ScbSystems);
            Property(x => x.Category);
            Property(x => x.StartDate);
            Property(x => x.EndDate);

            Bag(x => x.FileColumns, colmap => { }, map => map.OneToMany());
            Bag(x => x.FileMappings, colmap => { }, map => map.OneToMany());
            Bag(x => x.FileSchedulers, colmap => { }, map => map.OneToMany());
            //Bag(x => x.FilterConditions, colmap => { }, map => map.OneToMany());
        }
    }
}
