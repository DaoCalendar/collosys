using System.Collections.Generic;
using AngularUI.Shared.Model;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;

namespace AngularUI.Shared.webapis
{
    public class FetchEnums
    {
        public void AddQueryEnums(EnumList list)
        {
            var session = SessionManager.GetCurrentSession();
            var aliasList = session.QueryOver<FileDetail>().Select(x => x.AliasName).List<string>();
            list.AddToList("AliasNames", aliasList);

            var productList = session.QueryOver<ProductConfig>().Select(x => x.Product).List<string>();
            list.AddToList("Product", productList);
        }

        public void AddCustomEnums(EnumList list)
        {
            string[] dateList = { "dd-mm-yyyy", "yyyy-mm-dd", "mm-dd-yyyy" };
            list.AddToList("DateFormats", new List<string>(dateList));
        }

        public void InitSystemEnums(EnumList list)
        {
            list.AddToList(typeof(ScbEnums.ScbSystems));
            list.AddToList(typeof(ScbEnums.Products));
            list.AddToList(typeof(ScbEnums.Category));
            list.AddToList(typeof(ScbEnums.ClientDataTables));

            list.AddToList(typeof(ColloSysEnums.GridScreenName));
            list.AddToList(typeof(ColloSysEnums.BasicValueTypes));
            list.AddToList(typeof(ColloSysEnums.FileUploadBy));
            list.AddToList(typeof(ColloSysEnums.FileAliasName));
            list.AddToList(typeof(ColloSysEnums.Operators));
            list.AddToList(typeof(ColloSysEnums.FileDataType));
            list.AddToList(typeof(ColloSysEnums.FileMappingValueType));
            list.AddToList(typeof(ColloSysEnums.FileFrequency));
            list.AddToList(typeof(ColloSysEnums.FileType));
            list.AddToList(typeof(ColloSysEnums.ApproveStatus));
            list.AddToList(typeof(ColloSysEnums.BillStatus));
            list.AddToList(typeof(ColloSysEnums.ErrorStatus));
            list.AddToList(typeof(ColloSysEnums.AllocationPolicy));
            list.AddToList(typeof(ColloSysEnums.BillingPolicy));
            list.AddToList(typeof(ColloSysEnums.PaymentSource));
            list.AddToList(typeof(ColloSysEnums.UploadStatus));
            list.AddToList(typeof(ColloSysEnums.ReportingLevel));
            list.AddToList(typeof(ColloSysEnums.Permissions));
            list.AddToList(typeof(ColloSysEnums.HtmlInputType));
            list.AddToList(typeof(ColloSysEnums.Gender));
            list.AddToList(typeof(ColloSysEnums.DataProcessStatus));
            list.AddToList(typeof(ColloSysEnums.AllocBillingStatus));
            list.AddToList(typeof(ColloSysEnums.UsedFor));
            list.AddToList(typeof(ColloSysEnums.ConditionRelations));
            list.AddToList(typeof(ColloSysEnums.AllocationType));
            list.AddToList(typeof(ColloSysEnums.DelqFlag));
            list.AddToList(typeof(ColloSysEnums.DelqAccountStatus));
            list.AddToList(typeof(ColloSysEnums.AllocStatus));
            list.AddToList(typeof(ColloSysEnums.ChangeAllocReason));
            list.AddToList(typeof(ColloSysEnums.ListOfEnums));
            list.AddToList(typeof(ColloSysEnums.BillingStatus));
            list.AddToList(typeof(ColloSysEnums.ConditionType));
            list.AddToList(typeof(ColloSysEnums.PayoutSubpolicyType));
            list.AddToList(typeof(ColloSysEnums.PayoutLRType));
            list.AddToList(typeof(ColloSysEnums.Lsqlfunction));
            list.AddToList(typeof(ColloSysEnums.OutputType));
            list.AddToList(typeof(ColloSysEnums.CityCategory));
        }
    }
}