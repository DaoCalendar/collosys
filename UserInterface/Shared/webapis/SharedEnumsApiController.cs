using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Enumerations;
using AngularUI.Shared.Model;

namespace AngularUI.Shared.webapis
{
    public class SharedEnumsApiController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage FetchAllEnum()
        {
            var list = new EnumList();
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

            var enumDataList = new List<EnumData>();
            foreach (var @enum in list.enums)
            {
                var enumdata = new EnumData();
                enumdata.Name = @enum.Key;
                enumdata.Value = @enum.Value;
                enumDataList.Add(enumdata);
            }

            return Request.CreateResponse(HttpStatusCode.OK, enumDataList);

        }

    }
}
