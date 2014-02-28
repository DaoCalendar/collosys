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
    public class RInfoViewModel : Entity
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
        public decimal? LoanAmount { get; set; }
        public UInt32 Tenure { get; set; }
        public UInt32 Cycle { get; set; }
        public decimal? InterestPct { get; set; }
        public decimal? Emi { get; set; }
        public string Cluster { get; set; }
        public UInt32? Pincode { get; set; }
        public bool DoAllocate { get; set; }
        public DateTime FileDate { get; set; }

        public decimal InterestCharge { get; set; }
        public decimal FeeCharge { get; set; }
        public decimal BounceCharge { get; set; }
        public decimal LateCharge { get; set; }

        public UInt32 Bucket { get; set; }
        public decimal TotalDue { get; set; }
        public decimal CurrentDue { get; set; }
        public decimal EprincipalDue { get; set; }
        public decimal LprincipalDue { get; set; }
        public bool IsImpaired { get; set; }

        //public Guid RInfoId { get; set; }
        //public Guid RLinerId { get; set; }

        #endregion

        #region Query

        //public static IQueryable<RInfoViewModel> GetAllData()
        //{
        //    var session = SessionManager.GetCurrentSession();
        //    var result = session.QueryOver<RLiner>()
        //                        .Fetch(x => x.RInfo).Eager
        //        //.List()
        //                        .OrderBy(x => x.RInfo.Id).Asc.ThenBy(x => x.Version).Desc
        //                        .List<RLiner>()
        //                        .GroupBy(x => x.RInfo.Id).Select(g => g.First())
        //                        .OrderByDescending(x => x.CreatedOn)
        //                        .Select(x => new RInfoViewModel()
        //                            {
        //                                //RInfoId = x.RInfo.Id,
        //                                //RLinerId = x.Id,
        //                                StartDate = x.RInfo.StartDate ?? null,
        //                                EndDate = x.RInfo.EndDate ?? null,

        //                                Subproduct = x.RInfo.Products,

        //                                DelqStatus = x.RInfo.DelqStatus,
        //                                DelqAmount = x.RInfo.DelqAmount,
        //                                DelqDate = x.RInfo.DelqDate,

        //                                LoanNo = x.RInfo.AccountNo,
        //                                CustomerName = x.RInfo.CustomerName,
        //                                LoanAmount = x.RInfo.LoanAmount,
        //                                Tenure = x.RInfo.Tenure,
        //                                Cycle = x.RInfo.Cycle,
        //                                InterestPct = x.RInfo.InterestPct,
        //                                Emi = x.RInfo.Emi,
        //                                Cluster = x.RInfo.Cluster,
        //                                Pincode = x.RInfo.Pincode,
        //                                DoAllocate = x.RInfo.DoAllocate,
        //                                FileDate = x.FileDate,

        //                                InterestCharge = x.InterestCharge,
        //                                BounceCharge = x.BounceCharge,
        //                                FeeCharge = x.FeeCharge,
        //                                LateCharge = x.LateCharge,

        //                                Bucket = x.Bucket,
        //                                CurrentDue = x.CurrentDue,
        //                                TotalDue = x.TotalDue,
        //                                EprincipalDue = x.EprincipalDue,
        //                                LprincipalDue = x.LprincipalDue,
        //                                IsImpaired = x.IsImpaired,

        //                            }).ToList();
        //    return result.AsQueryable();
        //}

        #endregion

    }
}