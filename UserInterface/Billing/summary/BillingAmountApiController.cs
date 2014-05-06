using System.IO;
using AngularUI.Shared.webapis;
using ColloSys.DataLayer.Billing;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.Generic;
using ColloSys.Shared.SharedUtils;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using OfficeOpenXml;

#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.QueryBuilder.BillingBuilder;
using ColloSys.QueryBuilder.StakeholderBuilder;
using ColloSys.UserInterface.Areas.Billing.ViewModels;

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
        
        public IEnumerable<string> GetProducts()
        {
            return Enum.GetNames(typeof(ScbEnums.Products)).Where(x => x != "UNKNOWN")
                .ToList();
        }

        [HttpGet]
        
        public IEnumerable<Stakeholders> GetStakeholders(ScbEnums.Products product)
        {

            Stakeholders stakeholders = null;
            StkhWorking workings = null;
            StkhHierarchy hierarchy = null;
            StkhPayment payment = null;
            var session = SessionManager.GetCurrentSession();
            var data = session.QueryOver(() => stakeholders)
                              .Fetch(x => x.StkhWorkings).Eager
                              .Fetch(x => x.StkhPayments).Eager
                              .Fetch(x => x.Hierarchy).Eager
                              .JoinAlias(() => stakeholders.StkhPayments, () => payment, JoinType.LeftOuterJoin)
                              .JoinAlias(() => stakeholders.StkhWorkings, () => workings, JoinType.LeftOuterJoin)
                              .JoinAlias(() => stakeholders.Hierarchy, () => hierarchy,
                                         JoinType.LeftOuterJoin)
                              .Where(() => workings.Products == product)
                              .TransformUsing(Transformers.DistinctRootEntity)
                              .List();
            data.ForEach(x =>
            {
                x.Allocs = null;
                x.AllocSubpolicies = null;
            });
            return data;

        }

        [HttpGet]
        
        public BillAmount GetBillingData(ScbEnums.Products products, Guid stakeId, int month)
        {
            return BillAmountBuilder.OnStakeProductMonth(products, stakeId, month);
        }

        [HttpPost]
        
        public BillAmount ApproveBillingAmount(BillAmount billAmount)
        {
            BillAmountBuilder.Save(billAmount);
            return billAmount;
        }

        [HttpGet]
        
        public IEnumerable<BillDetail> GetBillingDetailData(ScbEnums.Products products, Guid stakeId, int month)
        {
            return BillDetailBuilder.OnStakeProductMonth(products, stakeId, month);
        }


        [HttpPost]
        
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
            BillAdhocBuilder.Save(finalBill.billAdhoc);
            BillDetailBuilder.Save(newBillDetail);
            BillAmountBuilder.Save(finalBill.billAmount);
            return finalBill;
        }

        // TODO : ICICI demo
        #region ICICI demo
        [HttpGet]
        [HttpTransaction2]
        public string ExcelForBillSammary(ScbEnums.Products products, Guid stakeId, int month)
        {
            var billAmount = Session.QueryOver<BillAmount>()
               .Where(x => x.Stakeholder.Id == stakeId)
               .And(x => x.Products == products)
               .And(x => x.Month == month)
               .SingleOrDefault();

            var billDetails = Session.QueryOver<BillDetail>()
                                   .Fetch(x => x.BillingPolicy).Eager
                                   .Fetch(x => x.BillingSubpolicy).Eager
                                   .Fetch(x => x.PaymentSource).Eager
                                   .Fetch(x => x.CustBillViewModels).Eager
                                   .Where(x => x.Stakeholder.Id == stakeId)
                                   .And(x => x.Products == products)
                                   .And(x => x.BillMonth == month)
                                   .And(x => x.PaymentSource == ColloSysEnums.PaymentSource.Variable)
                                   .TransformUsing(Transformers.DistinctRootEntity)
                                   .List();

            if (billAmount == null)
                return string.Empty;

            var file = new FileInfo(Path.GetTempPath() + "\\BillSummary.xlsx");
            var excelRowCounter = 1;
            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("StakeholderBillSummary");
                WriteBillSammary(billAmount, ws, ref excelRowCounter);

                foreach (var billDetail in billDetails)
                {
                    if (billDetail.Amount <= 0)
                        continue;

                    var wrCustBillViewModels = billDetail.CustBillViewModels.ToList();
                    WriteBillDetails(billDetail, wrCustBillViewModels[0].ConditionSatisfy, ws, ref excelRowCounter);

                    WriteCustBillViewModels(wrCustBillViewModels, ws, ref excelRowCounter);
                }

                pck.SaveAs(file);
            }

            return file.FullName;
        }

        private void WriteBillSammary(BillAmount billAmount, ExcelWorksheet ws, ref int excelRowCounter)
        {
            excelRowCounter++;

            var startRow = excelRowCounter;

            // stakeholder name
            ws.Cells[excelRowCounter, 2].Value = "Stakeholder Name";
            ws.Cells[excelRowCounter, 3].Value = billAmount.Stakeholder.Name;
            excelRowCounter++;


            // Month
            ws.Cells[excelRowCounter, 2].Value = "Month";
            ws.Cells[excelRowCounter, 3].Value = new DateTime((int)billAmount.Month / 100, (int)billAmount.Month % 100, 1).ToString("MMM-yyyy");
            excelRowCounter++;

            // Product
            ws.Cells[excelRowCounter, 2].Value = "Product";
            ws.Cells[excelRowCounter, 3].Value = billAmount.Products;
            excelRowCounter++;

            // Fixed Pay
            ws.Cells[excelRowCounter, 2].Value = "Fixed Pay";
            ws.Cells[excelRowCounter, 3].Value = billAmount.FixedAmount;
            excelRowCounter++;

            // Adhoc Pay
            ws.Cells[excelRowCounter, 2].Value = "Adhoc Pay";
            ws.Cells[excelRowCounter, 3].Value = billAmount.Deductions;
            excelRowCounter++;

            // Variable Pay
            ws.Cells[excelRowCounter, 2].Value = "Variable Pay";
            ws.Cells[excelRowCounter, 3].Value = billAmount.VariableAmount;
            excelRowCounter++;

            // Holding Payment
            ws.Cells[excelRowCounter, 2].Value = "Holding Payment";
            ws.Cells[excelRowCounter, 3].Value = billAmount.HoldAmount;
            excelRowCounter++;

            // Hol Release
            ws.Cells[excelRowCounter, 2].Value = "Hold Release";
            ws.Cells[excelRowCounter, 3].Value = billAmount.HoldRepayment;
            excelRowCounter++;

            // Variable Pay
            ws.Cells[excelRowCounter, 2].Value = "Total Payout";
            ws.Cells[excelRowCounter, 3].Value = billAmount.TotalAmount;
            var endRow = excelRowCounter;
            excelRowCounter++;

            ws.Cells[startRow, 2, endRow, 2].Style.Font.Bold = true;
        }

        private void WriteBillDetails(BillDetail billDetail, string condition, ExcelWorksheet ws, ref int excelRowCounter)
        {
            excelRowCounter++;

            var startRow = excelRowCounter;
            // subpolicy name
            ws.Cells[excelRowCounter, 2].Value = "Subpolicy Name";
            ws.Cells[excelRowCounter, 3].Value = billDetail.BillingSubpolicy.Name;
            excelRowCounter++;

            // subpolicy amount
            ws.Cells[excelRowCounter, 2].Value = "Subpolicy Amount";
            ws.Cells[excelRowCounter, 3].Value = billDetail.Amount;
            excelRowCounter++;

            // subpolicy amount
            ws.Cells[excelRowCounter, 2].Value = "Subpolicy Description";
            ws.Cells[excelRowCounter, 3].Value = condition;
            var endRow = excelRowCounter;
            excelRowCounter++;

            ws.Cells[startRow, 2, endRow, 2].Style.Font.Bold = true;
        }

        private void WriteCustBillViewModels(IEnumerable<ICustBillViewModel> listdata, ExcelWorksheet ws, ref int excelRowCounter)
        {
            //Create the worksheet

            if (listdata == null)
            {
                ws.Cells[excelRowCounter, 2].Value = "No Data";
                return;
            }

            // get properties to write
            var entityAllPropsList = typeof(ICustBillViewModel).GetProperties();

            //get our column headings
            for (var i = 0; i < entityAllPropsList.Length; i++)
            {
                ws.Cells[excelRowCounter, i + 2].Value = entityAllPropsList[i].Name;
            }

            ws.Cells[excelRowCounter, 2, excelRowCounter, entityAllPropsList.Length + 1].Style.Font.Bold = true;
            excelRowCounter++;

            //populate our Data
            foreach (var entity in listdata)
            {
                var propertyCounter = 2;
                for (var i = 0; i < entityAllPropsList.Length; i++)
                {
                    if (entityAllPropsList[i].PropertyType == typeof(DateTime) || entityAllPropsList[i].PropertyType == typeof(DateTime?))
                        ws.Cells[excelRowCounter, propertyCounter].Style.Numberformat.Format = "yyyy-mm-dd";

                    ws.Cells[excelRowCounter, propertyCounter].Value
                        = ReflectionUtil.GetPropertyValue(entity, entityAllPropsList[i].Name);
                    propertyCounter = propertyCounter + 1;
                }
                excelRowCounter = excelRowCounter + 1;
            }
        }

        #endregion
    }
}

//[HttpPost]
//
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