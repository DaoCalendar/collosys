#region references

using ColloSys.DataLayer.BaseEntity;
using ColloSys.DataLayer.Infra.Domain;
using NHibernate;
#endregion

namespace ColloSys.DataLayer.Infra.Mapping
{
    public class WebEventMap : EntityMap<WebEvent>
    {
        public WebEventMap()
        {
            #region properties
            Property(x => x.EventTimeUtc, map =>
            {
                map.NotNullable(true);
                map.Type(NHibernateUtil.DateTime);
            });

            Property(x => x.EventTime, map =>
            {
                map.NotNullable(true);
                map.Type(NHibernateUtil.DateTime);
            });
            Property(x => x.EventCode);
            Property(x => x.EventType);
            Property(x => x.EventSequence);
            Property(x => x.EventOccurrence);
            Property(x => x.EventDetailCode);
            Property(x => x.Message);
            Property(x => x.ApplicationPath);
            Property(x => x.ApplicationVirtualPath);
            Property(x => x.MachineName);
            Property(x => x.RequestUrl);
            Property(x => x.ExceptionType);
            Property(x => x.Details);
            #endregion
        }
    }
}

/* no delete frm amol
            [EventId] [char](32) NOT NULL,
            [EventTimeUtc] [datetime] NOT NULL,
            [EventTime] [datetime] NOT NULL,
            [EventType] [nvarchar](256) NOT NULL,
            [EventSequence] [decimal](19, 0) NOT NULL,
            [EventOccurrence] [decimal](19, 0) NOT NULL,
            [EventCode] [int] NOT NULL,
            [EventDetailCode] [int] NOT NULL,
            [Message] [nvarchar](1024) NULL,
            [ApplicationPath] [nvarchar](256) NULL,
            [ApplicationVirtualPath] [nvarchar](256) NULL,
            [MachineName] [nvarchar](256) NOT NULL,
            [RequestUrl] [nvarchar](1024) NULL,
            [ExceptionType] [nvarchar](256) NULL,
            [Details] [ntext] NULL,
 */
