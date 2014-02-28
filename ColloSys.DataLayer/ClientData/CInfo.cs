using System;
using System.Collections.Generic;
using ColloSys.DataLayer.Allocation;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.SharedDomain;
using NHibernate.Mapping;

namespace ColloSys.DataLayer.ClientData
{
    public class CInfo : SharedInfo
    {
        
        public virtual decimal CreditLimit { get; set; }
        public virtual decimal UnbilledDue { get; set; }
        public virtual decimal CurrentBalance { get; set; }
        public virtual uint PeakBucket { get; set; }
        public virtual ColloSysEnums.DelqAccountStatus AccountStatus { get; set; }
        public virtual decimal BucketAmount { get; set; }
        public virtual string Block { get; set; }
        public virtual string AltBlock { get; set; }
        public virtual decimal LastPayAmount { get; set; }
        public virtual DateTime? LastPayDate { get; set; }
        public virtual decimal CurrentDue { get; set; }
        public virtual decimal Bucket0Due { get; set; }
        public virtual decimal Bucket1Due { get; set; }
        public virtual decimal Bucket2Due { get; set; }
        public virtual decimal Bucket3Due { get; set; }
        public virtual decimal Bucket4Due { get; set; }
        public virtual decimal Bucket5Due { get; set; }
        public virtual decimal Bucket6Due { get; set; }
        public virtual decimal Bucket7Due { get; set; }
        public virtual decimal OutStandingBalance { get; set; }
        public virtual string Location { get; set; }
        public virtual string DelqHistoryString { get; set; }
        public virtual decimal CustTotalDue { get; set; }
        public virtual DateTime FileDate { get; set; }
        public virtual ulong FileRowNo { get; set; }

        public virtual FileScheduler FileScheduler { get; set; }

        public virtual Iesi.Collections.Generic.ISet<CAlloc> CAllocs { get; set; }
       
    }
}
