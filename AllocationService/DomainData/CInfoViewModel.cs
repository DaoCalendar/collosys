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
    public class CInfoViewModel : Entity
    {
        #region Properties

        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public EnumHelper.ScbSystems Product { get; set; }
        public string Subproduct { get; set; }

        public string DelqStatus { get; set; }
        public DateTime DelqDate { get; set; }
        public decimal DelqAmount { get; set; }

        public UInt64 CardNo { get; set; }
        public UInt64? CustId { get; set; }
        public string CustomerName { get; set; }
        public UInt64? CreditLimit { get; set; }
        public UInt16 Cycle { get; set; }
        public decimal InterestPct { get; set; }
        public string Cluster { get; set; }
        public UInt64? Pincode { get; set; }
        public bool DoAllocate { get; set; }
        public UInt32 Bucket { get; set; }
        public decimal TotalDue { get; set; }
        public decimal CurrentDue { get; set; }
        public decimal Bucket1Due { get; set; }
        public decimal Bucket2Due { get; set; }
        public decimal Bucket3Due { get; set; }
        public decimal Bucket4Due { get; set; }
        public decimal Bucket5Due { get; set; }
        public decimal Bucket6Due { get; set; }
        public decimal UnbilledAmount { get; set; }
        public decimal LastPayAmount { get; set; }
        public DateTime? LastPayDate { get; set; }
        public decimal CustTotalDue { get; set; }
        public string Block { get; set; }
        public string AltBlock { get; set; }
        public DateTime FileDate { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Query

        //public static IQueryable<CInfoViewModel> GetAllData()
        //{
        //    var session = SessionManager.GetCurrentSession();
        //    var result = session.QueryOver<CLiner>()
        //                        .Fetch(x => x.CInfo)
        //                        .Eager
        //                        .List()
        //                        .Select(x => new CInfoViewModel
        //                            {
        //                                StartDate = x.CInfo.StartDate,
        //                                EndDate = x.CInfo.EndDate,

        //                                Subproduct = x.CInfo.Products,

        //                                DelqStatus = x.CInfo.DelqStatus,
        //                                DelqAmount = x.CInfo.DelqAmount,
        //                                DelqDate = x.CInfo.DelqDate,

        //                                CardNo = x.CInfo.AccountNo,
        //                                CustId = x.CInfo.CustId,
        //                                CustomerName = x.CInfo.CustomerName,
        //                                CreditLimit = x.CInfo.CreditLimit,
        //                                Cycle = (ushort)x.CInfo.Cycle,
        //                                InterestPct = x.CInfo.InterestPct,
        //                                Cluster = x.CInfo.Cluster,
        //                                Pincode = x.CInfo.Pincode,
        //                                DoAllocate = x.CInfo.DoAllocate,

        //                                Bucket = x.Bucket,
        //                                Bucket1Due = x.Bucket1Due,
        //                                Bucket2Due = x.Bucket2Due,
        //                                Bucket3Due = x.Bucket3Due,
        //                                Bucket4Due = x.Bucket4Due,
        //                                Bucket5Due = x.Bucket5Due,
        //                                Bucket6Due = x.Bucket6Due,
        //                                UnbilledAmount = x.UnbilledAmount,
        //                                LastPayAmount = x.LastPayAmount,
        //                                LastPayDate = x.LastPayDate,
        //                                CustTotalDue = x.CustTotalDue,
        //                                Block = x.Block,
        //                                AltBlock = x.AltBlock,
        //                                FileDate = x.FileDate
        //                            })
        //                        .ToList();

        //    return result.AsQueryable();
        //}

        #endregion

        
        public static void CLinerQuery()
        {
            //RLS Write-Off Query
            var rlsWriteOffQuery = @"Select" +
                                   "CustomerName as RInfo_CustomerName," +
                                   "Cycle as RInfo_Cycle, Pincode as RInfo_Pincode," +
                                   "DoAllocate as RInfo_DoAllocate, DelqDate as RInfo_DelqDate," +
                                   "TotalDue as RWriteOff_TotalDue," +
                                   "IsSetteled as RWriteOff_IsSetteled," +
                                   "ActivityCode as CACS_ActivityCode, ExcuseCode as CACS_ExcuseCode" +
                                   "from  R_INFO, R_WRITEOFF , CACS_ACTIVITY" +
                                   "where R_INFO.RInfoId=R_WRITEOFF.RInfoId and" +
                                   "R_INFO.AccountNo = CACS_ACTIVITY.AccountNumber";

            //RLS Liner Query
            var rlsLinerQuery = @"Select" +
                                "AccountNo as RInfo_AccountNo," +
                                "CustomerName as RInfo_CustomerName," +
                                "Cycle as RInfo_Cycle, Pincode as RInfo_Pincode," +
                                "DoAllocate as RInfo_DoAllocate, DelqDate as RInfo_DelqDate," +
                                "DelqAmount as RInfo_DelqAmount," +
                                "TotalDue as RLiner_TotalDue, IsImpaired as RLiner_IsImpaired," +
                                "Bucket as RLiner_Bucket," +
                                "ActivityCode as CACS_ActivityCode, ExcuseCode as CACS_ExcuseCode" +
                                "from  R_INFO, R_LINER, CACS_ACTIVITY" +
                                "where R_INFO.RInfoId = R_LINER.RInfoId and" +
                                "R_INFO.AccountNo = CACS_ACTIVITY.AccountNumber";

            //EBBS Write-Off Query
            var ebbsWriteOffQuery = @"Select" +
                                    "CustomerName as EInfo_CustomerName," +
                                    "Cycle as EInfo_Cycle, Pincode as EInfo_Pincode," +
                                    "DoAllocate as EInfo_DoAllocate, DelqDate as EInfo_DelqDate," +
                                    "TotalDue as EWriteOff_TotalDue," +
                                    "CurrentDue as EWriteOff_CurrentDue, IsSetteled as EWriteOff_IsSetteled," +
                                    "ActivityCode as CACS_ActivityCode, ExcuseCode as CACS_ExcuseCode" +
                                    "from  E_INFO, E_WRITEOFF , CACS_ACTIVITY" +
                                    "where E_INFO.EInfoId=E_WRITEOFF.EInfoId and" +
                                    "E_INFO.AccountNo = CACS_ACTIVITY.AccountNumber";

            //EBBS Liner Query
            var ebbsLinerQuery = @"Select " +
                                 "AccountNo as EInfo_AccountNo," +
                                 "CustomerName as EInfo_CustomerName," +
                                 "Cycle as EInfo_Cycle, Pincode as EInfo_Pincode," +
                                 "DoAllocate as EInfo_DoAllocate, DelqDate as EInfo_DelqDate," +
                                 "DelqAmount as EInfo_DelqAmount," +
                                 "TotalDue as ELiner_TotalDue," +
                                 "Bucket as ELiner_Bucket, MinimumDue as ELiner_MinimumDue," +
                                 "ActivityCode as CACS_ActivityCode, ExcuseCode as CACS_ExcuseCode" +
                                 "from  E_INFO, E_LINER, CACS_ACTIVITY" +
                                 "where E_INFO.EInfoId = E_LINER.EInfoId and" +
                                 "E_INFO.AccountNo = CACS_ACTIVITY.AccountNumber";

            //CCMS Write-Off Query
            var ccmsWriteOffQuery = @"Select " +
                                    "CustId as CInfo_CustID, " +
                                    "CustomerName as CInfo_CustomerName," +
                                    "Cycle as CInfo_Cycle, Pincode as CInfo_Pincode," +
                                    "DoAllocate as CInfo_DoAllocate, DelqDate as CInfo_DelqDate," +
                                    "TotalDue as CWriteOff_TotalDue," +
                                    "LastPayDate as CWriteOff_LastPayDate, Block as CWriteOff_Block," +
                                    "AltBlock as CWriteOff_AltBlock," +
                                    "ActivityCode as CACS_ActivityCode, ExcuseCode as CACS_ExcuseCode" +
                                    "from  C_INFO, C_WRITEOFF , CACS_ACTIVITY" +
                                    "where C_INFO.CInfoId=C_WRITEOFF.CInfoId and" +
                                    "C_INFO.AccountNo = CACS_ACTIVITY.AccountNumber";
            //CCMS liner query
            var ccmsLinerQuery = @"Select " +
                                 "CustId as CInfo_CustID" +
                                 "CustomerName as CInfo_CustomerName," +
                                 "Cycle as CInfo_Cycle, Pincode as CInfo_Pincode," +
                                 "DoAllocate as CInfo_DoAllocate, DelqDate as CInfo_DelqDate," +
                                 "TotalDue as CLiner_TotalDue, LastPayAmount as CLiner_LastPayAmount," +
                                 "LastPayDate as CLiner_LastPayDate, Block as CLiner_Block," +
                                 "AltBlock as CLiner_AltBlock," +
                                 "ActivityCode as CACS_ActivityCode, ExcuseCode as CACS_ExcuseCode" +
                                 "from  C_INFO, C_LINER , CACS_ACTIVITY" +
                                 "where C_INFO.CInfoId=C_LINER.CInfoId and" +
                                 "C_INFO.AccountNo = CACS_ACTIVITY.AccountNumber";


            var session = SessionManager.GetCurrentSession();
        }

    }
}


//return session.QueryOver<CLiner>()
//              .JoinQueryOver<CInfo>(j => j.CInfo)
//              .OrderBy(x => x.Id).Asc.ThenBy(x => x.Version).Desc
//              .List<CLiner>()
//              .GroupBy(x => x.FileScheduler.Id).Select(g => g.First())
//              .Select(x => new CInfoViewModel
//                  {
//                      StartDate = x.CInfo.DateRange.StartDate,
//                      EndDate = x.CInfo.DateRange.EndDate,

//                      Product = x.CInfo.Product.Product,
//                      Subproduct = x.CInfo.Product.Subproduct,

//                      DelqStatus = x.CInfo.Delq.DelqStatus,
//                      DelqAmount = x.CInfo.Delq.DelqAmount,
//                      DelqDate = x.CInfo.Delq.DelqDate,

//                      CardNo = x.CInfo.CardNo,
//                      CustId = x.CInfo.CustId,
//                      CustomerName = x.CInfo.CustomerName,
//                      CreditLimit = x.CInfo.CreditLimit,
//                      Cycle = x.CInfo.Cycle,
//                      InterestPct = x.CInfo.InterestPct,
//                      Cluster = x.CInfo.Cluster,
//                      Pincode = x.CInfo.Pincode,
//                      DoAllocate = x.CInfo.DoAllocate,

//                      Bucket = x.Bucket,
//                      Bucket1Due = x.Bucket1Due,
//                      Bucket2Due = x.Bucket2Due,
//                      Bucket3Due = x.Bucket3Due,
//                      Bucket4Due = x.Bucket4Due,
//                      Bucket5Due = x.Bucket5Due,
//                      Bucket6Due = x.Bucket6Due,
//                      UnbilledAmount = x.UnbilledAmount,
//                      LastPayAmount = x.LastPayAmount,
//                      LastPayDate = x.LastPayDate,
//                      CustTotalDue = x.CustTotalDue,
//                      Block = x.Block,
//                      AltBlock = x.AltBlock,
//                      FileDate = x.FileDate

//                  }).ToList();
