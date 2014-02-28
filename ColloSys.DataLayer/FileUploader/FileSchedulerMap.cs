#region References

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Domain;
using NHibernate;
#endregion

namespace ColloSys.DataLayer.Mapping
{
    public class FileSchedulerMap : EntityMap<FileScheduler>
    {
        public FileSchedulerMap()
        {
            Table("FILE_SCHEDULER");

            #region     properties

            Property(x => x.FileName, map => map.UniqueKey("UK_FILESHEDULER"));
            Property(x => x.FileServer);
            Property(x => x.FileDirectory);
            Property(x => x.FileSize);
            Property(x => x.StatusDescription, map => map.NotNullable(false));
            Property(x => x.IsImmediate);
            Property(x => x.UploadStatus);
            Property(x => x.FileDate, map =>
            {
                map.UniqueKey("UK_FILESHEDULER");
                map.Index("IX_FILESHEDULER");
            });
            Property(x => x.TotalRows);
            Property(x => x.ValidRows);
            Property(x => x.ErrorRows);
            Property(x => x.ImmediateReason, map => map.NotNullable(false));
            Property(x => x.AllocBillDone);

            #region DateRange
            Property(x => x.StartDateTime, map => map.NotNullable(true));
            Property(x => x.EndDateTime);
            #endregion

            #endregion

            #region Category Component

            Property(x => x.ScbSystems);
            Property(x => x.Category);

            #endregion

            #region Mapping
            ManyToOne(x => x.FileDetail, map =>
            {
                map.NotNullable(true);
                map.UniqueKey("UK_FILESHEDULER");
                map.Index("IX_FILESHEDULER");
            });


            Set(x => x.FileStatuss, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.CLiners, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.CUnbilleds, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.CPayments, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.CWriteoffs, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.CacsActivities, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.ELiners, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.EPayments, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.EWriteoffs, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.RLiners, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.RPayments, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            Set(x => x.RWriteoffs, colmap =>
            {
            }, map => map.OneToMany(x =>
            {
            }));
            #endregion
        }
    }
}
