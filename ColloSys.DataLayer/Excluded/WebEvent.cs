#region References
using System;

#endregion
namespace ColloSys.DataLayer.Infra.Domain
{
    public class WebEvent : Entity 
    {
//        public Guid EventId { get; set; }

        public virtual DateTime? EventTimeUtc { get; set; }
        public virtual DateTime? EventTime { get; set; }
        public virtual string EventType { get; set; }
        public virtual decimal EventSequence { get; set; }
        public virtual decimal EventOccurrence { get; set; }
        public virtual int EventCode { get; set; }
        public virtual int EventDetailCode { get; set; }
        public virtual string Message { get; set; }
        public virtual string ApplicationPath { get; set; }
        public virtual string ApplicationVirtualPath { get; set; }
        public virtual string MachineName { get; set; }
        public virtual string RequestUrl { get; set; }
        public virtual string ExceptionType { get; set; }
        public virtual string Details { get; set; }
    }
}


/*
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
            [Details] [ntext] NULL,*/
