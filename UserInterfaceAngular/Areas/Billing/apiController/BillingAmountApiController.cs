using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Billing.ViewModels;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;

//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class BillingAmountApiController : BaseApiController<BillAdhoc>
    {
        private static readonly StakeQueryBuilder StakeQuery=new StakeQueryBuilder();

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> GetProducts()
        {
            return Enum.GetNames(typeof(ScbEnums.Products)).Where(x => x != "UNKNOWN")
                .ToList();
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<Stakeholders> GetStakeholders(ScbEnums.Products product)
        {
            var data = StakeQuery.OnProduct(product);
            //Stakeholders stakeholders = null;
            //StkhWorking working = null;
            //StkhHierarchy hierarchy = null;
            //var session = SessionManager.GetCurrentSession();
            //var data = session.QueryOver<Stakeholders>(() => stakeholders)
            //                  .Fetch(x => x.StkhWorkings).Eager
            //                  .JoinQueryOver(() => stakeholders.StkhWorkings, () => working)
            //                  .JoinQueryOver(() => stakeholders.Hierarchy, () => hierarchy)
            //                  .Where(() => working.Products == product)
            //                  .And(() => hierarchy.IsInAllocation)
            //                  .And(() => hierarchy.IsInField)
            //                  .TransformUsing(Transformers.DistinctRootEntity)
            //                  .List<Stakeholders>();
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public BillAmount GetBillingData(ScbEnums.Products products, Guid stakeId, int month)
        {
            var billAmount = Session.QueryOver<BillAmount>()
                .Where(x => x.Stakeholder.Id == stakeId)
                .And(x => x.Products == products)
                .And(x => x.Month == month)
                .SingleOrDefault();
            return billAmount;
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public BillAmount ApproveBillingAmount(BillAmount billAmount)
        {
            Session.SaveOrUpdate(billAmount);
            return billAmount;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<BillDetail> GetBillingDetailData(ScbEnums.Products products, Guid stakeId, int month)
        {
            var billDetail = Session.QueryOver<BillDetail>()
                                    .Fetch(x => x.BillingPolicy).Eager
                                    .Fetch(x => x.BillingSubpolicy).Eager
                                    .Fetch(x => x.PaymentSource).Eager
                                    .Where(x => x.Stakeholder.Id == stakeId)
                                    .And(x => x.Products == products)
                                    .And(x => x.BillMonth == month)
                                    .List();
            return billDetail;
        }


        [HttpPost]
        [HttpTransaction(Persist = true)]
        public FinalBillData SaveBillAdhoc(FinalBillData finalBill)
        {
            var newBillDetail = new BillDetail();

            //finalBill.billAdhoc=new BillAdhoc();
            //finalBill.billAmount=new BillAmount();

            newBillDetail.BillMonth = finalBill.billAdhoc.StartMonth;
            newBillDetail.Amount = finalBill.billAdhoc.TotalAmount;
            newBillDetail.BillCycle = 0;
            newBillDetail.BillingPolicy = null;
            newBillDetail.BillingSubpolicy = null;
            newBillDetail.Products = finalBill.billAdhoc.Products;
            newBillDetail.Stakeholder = finalBill.billAdhoc.Stakeholder;
            newBillDetail.BillAdhoc = finalBill.billAdhoc;
            newBillDetail.Products = finalBill.billAmount.Products;
            newBillDetail.PaymentSource = ColloSysEnums.PaymentSource.Adhoc;
            var amount = finalBill.billAdhoc.IsCredit ? newBillDetail.Amount : (-1) * newBillDetail.Amount;

            finalBill.billAmount.Deductions = finalBill.billAmount.Deductions + amount;
            
            Session.SaveOrUpdate(newBillDetail);
            Session.SaveOrUpdate(finalBill.billAdhoc);
            Session.SaveOrUpdate(finalBill.billAmount);

            return finalBill;
        }




        //[HttpPost]
        //[HttpTransaction(Persist = true)]
        //public void SaveBillAdhoc(BillAdhoc billAdhoc,BillAmount billAmount)
        //{
        //    var newBillDetail = new BillDetail();
        //    newBillDetail.BillMonth = billAdhoc.StartMonth;
        //    newBillDetail.Amount = billAdhoc.TotalAmount;
        //    newBillDetail.BillCycle = 0;
        //    newBillDetail.BillingPolicy = null;
        //    newBillDetail.BillingSubpolicy = null;
        //    newBillDetail.Products = billAdhoc.Products;
        //    newBillDetail.Stakeholder = billAdhoc.Stakeholder;
        //    newBillDetail.BillAdhoc = billAdhoc;

        //    billAmount.Deductions = billAmount.Deductions + newBillDetail.Amount;
        //    Session.SaveOrUpdate(billAmount);
        //    Session.SaveOrUpdate(newBillDetail);
        //    Session.SaveOrUpdate(billAdhoc);
        //}

    }
}