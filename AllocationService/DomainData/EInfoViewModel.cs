#region References

using System;
using System.Linq;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Shared;

#endregion


namespace AllocationService.DomainData
{
    public class EInfoViewModel : Entity
    {
        #region Properties

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public EnumHelper.ScbSystems Product { get; set; }
        public string Subproduct { get; set; }

        public string DelqStatus { get; set; }
        public DateTime DelqDate { get; set; }
        public decimal DelqAmount { get; set; }

        public UInt64 LoanNo { get; set; }
        public string CustomerName { get; set; }
        public decimal? OdLimit { get; set; }
        public UInt32 Cycle { get; set; }
        public decimal? InterestPct { get; set; }
        public string Cluster { get; set; }
        public UInt32? Pincode { get; set; }
        public bool DoAllocate { get; set; }
        public DateTime FileDate { get; set; }

        public UInt32 Bucket { get; set; }
        public decimal TotalDue { get; set; }
        public decimal CurrentDue { get; set; }
        public decimal BucketDue { get; set; }
        public decimal Bucket1Due { get; set; }
        public decimal Bucket2Due { get; set; }
        public decimal Bucket3Due { get; set; }
        public decimal Bucket4Due { get; set; }
        public decimal Bucket5Due { get; set; }
        public decimal AmountRepaid { get; set; }
        public decimal InterestCharge { get; set; }
        public decimal FeeCharge { get; set; }
        public decimal MinimumDue { get; set; }

        #endregion

        #region Query

        //public static IQueryable<EInfoViewModel> GetAllData()
        //{
        //    var session = SessionManager.GetCurrentSession();
        //    return session.QueryOver<ELiner>()
        //                  .Fetch(x => x.EInfo).Eager
        //                  .List()
        //                  .Select(x => new EInfoViewModel()
        //                      {
        //                          StartDate = x.EInfo.StartDate ?? null,
        //                          EndDate = x.EInfo.EndDate ?? null,

        //                          Subproduct = x.EInfo.Products,

        //                          DelqStatus = x.EInfo.DelqStatus,
        //                          DelqAmount = x.EInfo.DelqAmount,
        //                          DelqDate = x.EInfo.DelqDate,

        //                          LoanNo = x.EInfo.AccountNo,
        //                          CustomerName = x.EInfo.CustomerName,
        //                          OdLimit = x.EInfo.OdLimit,
        //                          Cycle = x.EInfo.Cycle,
        //                          InterestPct = x.EInfo.InterestPct,
        //                          Cluster = x.EInfo.Cluster,
        //                          Pincode = x.EInfo.Pincode,
        //                          DoAllocate = x.EInfo.DoAllocate,
        //                          FileDate = x.FileDate,

        //                          Bucket = x.Bucket,
        //                          CurrentDue = x.CurrentDue,
        //                          TotalDue = x.TotalDue,
        //                          BucketDue = x.BucketDue,
        //                          Bucket1Due = x.Bucket1Due,
        //                          Bucket2Due = x.Bucket2Due,
        //                          Bucket3Due = x.Bucket3Due,
        //                          Bucket4Due = x.Bucket4Due,
        //                          Bucket5Due = x.Bucket5Due,
        //                          AmountRepaid = x.AmountRepaid,
        //                          InterestCharge = x.InterestCharge,
        //                          FeeCharge = x.FeeCharge,
        //                          MinimumDue = x.MinimumDue,

        //                      }).ToList<EInfoViewModel>().AsQueryable();
        //}

        #endregion

    }
}