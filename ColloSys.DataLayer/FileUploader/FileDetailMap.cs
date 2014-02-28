#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.FileUploader;
using NHibernate;
#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class FileDetailMap : EntityMap<FileDetail>
    {
        public FileDetailMap()
        {
            Table("FILE_DETAILS");

            #region Property

            Property(x => x.AliasName, map =>
            {
                map.UniqueKey("UK_FILE_DETAILS");
                map.Index("IX_FILE_DETAILS");
            });
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

            #endregion

            #region Relationship set

            Set(x => x.FileColumns, colmap => { }, map => map.OneToMany());
            Set(x => x.FileMappings, colmap => { }, map => map.OneToMany());
            Set(x => x.FileSchedulers, colmap => { }, map => map.OneToMany());

            #endregion

        }
    }
}
