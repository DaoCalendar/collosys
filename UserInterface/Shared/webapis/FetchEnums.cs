using System.Collections.Generic;
using AngularUI.Shared.Model;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.FileUploader;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.GenericBuilder;
using System.Linq;

namespace AngularUI.Shared.webapis
{
    public class FetchEnums
    {
        private static readonly ProductConfigBuilder ProductConfigBuilder=new ProductConfigBuilder(); 
        public void AddQueryEnums(EnumList list)
        {
            //TODO -SQL error occred
            var session = SessionManager.GetCurrentSession();
            //var aliasList = session.QueryOver<FileDetail>().Select(x => x.AliasName).List<string>();
            //list.AddToList("AliasNames", aliasList);

            var productList = ProductConfigBuilder.GetProductsString().ToList();
            list.AddToList("ProductEnum", productList);
        }

        public void AddCustomEnums(EnumList list)
        {
            string[] dateList = { "dd-MM-yyyy", "yyyy-MM-dd", "MM-dd-yyyy" };
            list.AddToList("DateFormat", new List<string>(dateList));

            string[] operatorList = { "Plus", "Minus", "Multiply", "Divide", "ModuloDivide" };
            list.AddToList("OperatorType", new List<string>(operatorList));

            string[] relationType = { "And", "Or" };
            list.AddToList("RelationType", new List<string>(relationType));

            string[] typeSwitch = { "Table", "Formula", "Matrix", "Value" };
            list.AddToList("TypeSwitch", new List<string>(typeSwitch));

            string[] dateValueEnum = { "First_Quarter", "Second_Quarter", "Third_Quarter", 
                                         "Fourth_Quarter", "Start_of_Year", "Start_of_Month", 
                                         "Start_of_Week", "Today", "End_of_Week", "End_of_Month", 
                                         "End_of_Year", "Absolute_Date" };
            list.AddToList("DateValueEnum", new List<string>(dateValueEnum));

            string[] lsqlfunctionType = { "Sum", "Count", "Avg" };
            list.AddToList("LsqlFunctionType", new List<string>(lsqlfunctionType));

            string[] textConditionOperators = { "EqualTo", "NotEqualTo", "Contains", "DoNotContains", "StartsWith", "EndsWith", "IsInList" };
            list.AddToList("TextConditionOperators", new List<string>(textConditionOperators));

            string[] checkboxConditionOperators = { "EqualTo" };
            list.AddToList("CheckboxConditionOperators", new List<string>(checkboxConditionOperators));

            string[] dropdownConditionOperators = { "EqualTo", "NotEqualTo" };
            list.AddToList("DropdownConditionOperators", new List<string>(dropdownConditionOperators));

            string[] conditionOperators = { "EqualTo", "NotEqualTo", "LessThan", "LessThanEqualTo", "GreaterThan", "GreaterThanEqualTo" };
            list.AddToList("ConditionOperators", new List<string>(conditionOperators));

            string[] vertical = { "Field", "Telecalling", "BackOffice" };
            list.AddToList("Vertical", new List<string>(vertical));
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
            list.AddToList(typeof(ColloSysEnums.BillPaymentStatus));
            list.AddToList(typeof(ColloSysEnums.TaxApplicableTo));
            list.AddToList(typeof(ColloSysEnums.TaxType));
            list.AddToList(typeof(ColloSysEnums.TaxApplyOn));
            list.AddToList(typeof(ColloSysEnums.ApplyOn));
            list.AddToList(typeof(ColloSysEnums.RuleForHolding));
            list.AddToList(typeof(ColloSysEnums.Activities));
            list.AddToList(typeof(ColloSysEnums.PolicyType));
            list.AddToList(typeof(ColloSysEnums.PolicyOn));

        }
    }
}