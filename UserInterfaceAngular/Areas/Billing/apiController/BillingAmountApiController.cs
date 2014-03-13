#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Billing.ViewModels;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;

#endregion


//stakeholders calls changed
namespace ColloSys.UserInterface.Areas.Billing.apiController
{
    public class BillingAmountApiController : BaseApiController<BillAdhoc>
    {
        private static readonly StakeQueryBuilder StakeQuery=new StakeQueryBuilder();
        private static readonly BillAmountBuilder BillAmountBuilder=new BillAmountBuilder();
        private static readonly BillDetailBuilder BillDetailBuilder=new BillDetailBuilder();
        private static readonly BillAdhocBuilder BillAdhocBuilder=new BillAdhocBuilder();

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
            return data;
        }

        [HttpGet]
        [HttpTransaction]
        public BillAmount GetBillingData(ScbEnums.Products products, Guid stakeId, int month)
        {
            return BillAmountBuilder.OnStakeProductMonth(products, stakeId, month);
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public BillAmount ApproveBillingAmount(BillAmount billAmount)
        {
            BillAmountBuilder.Save(billAmount);
            return billAmount;
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<BillDetail> GetBillingDetailData(ScbEnums.Products products, Guid stakeId, int month)
        {
            return BillDetailBuilder.OnStakeProductMonth(products, stakeId, month);
        }


        [HttpPost]
        [HttpTransaction(Persist = true)]
        public FinalBillData SaveBillAdhoc(FinalBillData finalBill)
        {
            var newBillDetail = new BillDetail
                {
                    BillMonth = finalBill.billAdhoc.StartMonth,
                    Amount = finalBill.billAdhoc.TotalAmount,
                    BillCycle = 0,
                    BillingPolicy = null,
                    BillingSubpolicy = null,
                    Products = finalBill.billAdhoc.Products,
                    Stakeholder = finalBill.billAdhoc.Stakeholder,
                    BillAdhoc = finalBill.billAdhoc
                };

            newBillDetail.Products = finalBill.billAmount.Products;
            newBillDetail.PaymentSource = ColloSysEnums.PaymentSource.Adhoc;
            var amount = finalBill.billAdhoc.IsCredit ? newBillDetail.Amount : (-1) * newBillDetail.Amount;

            finalBill.billAmount.Deductions = finalBill.billAmount.Deductions + amount;
            
            BillDetailBuilder.Save(newBillDetail);
            BillAdhocBuilder.Save(finalBill.billAdhoc);
            BillAmountBuilder.Save(finalBill.billAmount);
            return finalBill;
        }
    }
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