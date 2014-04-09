using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using ColloSys.DataLayer.Enumerations;

namespace AngularUI.FileUpload.Testcase
{
    [TestFixture]
    public class FetchEnumsTest
    {
        public class EnumList
        {
            public Dictionary<string, List<string>> enums { get; private set; }

            public EnumList()
            {
                enums = new Dictionary<string, List<string>>();
            }

            public void AddToList(Type enumtype)
            {
                if (enumtype.IsEnum)
                {
                    enums.Add(enumtype.Name, Enum.GetNames(enumtype).ToList());
                }
            }

        }

        [Test]
        public void TestEnum()
        {
            var scbEnumlist = new EnumList();
            scbEnumlist.AddToList(typeof (ScbEnums.ScbSystems));
            scbEnumlist.AddToList(typeof (ScbEnums.Products));
            scbEnumlist.AddToList(typeof (ScbEnums.Category));
            scbEnumlist.AddToList(typeof (ScbEnums.ClientDataTables));

            var collosysEnumList = new EnumList();
            collosysEnumList.AddToList(typeof (ColloSysEnums.GridScreenName));
            collosysEnumList.AddToList(typeof (ColloSysEnums.BasicValueTypes));
            collosysEnumList.AddToList(typeof (ColloSysEnums.FileUploadBy));
            collosysEnumList.AddToList(typeof (ColloSysEnums.FileAliasName));
            collosysEnumList.AddToList(typeof (ColloSysEnums.Operators));
            collosysEnumList.AddToList(typeof (ColloSysEnums.FileDataType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.FileMappingValueType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.FileFrequency));
            collosysEnumList.AddToList(typeof (ColloSysEnums.FileType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.ApproveStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.BillStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.ErrorStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.AllocationPolicy));
            collosysEnumList.AddToList(typeof (ColloSysEnums.BillingPolicy));
            collosysEnumList.AddToList(typeof (ColloSysEnums.PaymentSource));
            collosysEnumList.AddToList(typeof (ColloSysEnums.UploadStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.ReportingLevel));
            collosysEnumList.AddToList(typeof (ColloSysEnums.Permissions));
            collosysEnumList.AddToList(typeof (ColloSysEnums.HtmlInputType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.Gender));
            collosysEnumList.AddToList(typeof (ColloSysEnums.DataProcessStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.AllocBillingStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.UsedFor));
            collosysEnumList.AddToList(typeof (ColloSysEnums.ConditionRelations));
            collosysEnumList.AddToList(typeof (ColloSysEnums.AllocationType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.DelqFlag));
            collosysEnumList.AddToList(typeof (ColloSysEnums.DelqAccountStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.AllocStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.ChangeAllocReason));
            collosysEnumList.AddToList(typeof (ColloSysEnums.ListOfEnums));
            collosysEnumList.AddToList(typeof (ColloSysEnums.BillingStatus));
            collosysEnumList.AddToList(typeof (ColloSysEnums.ConditionType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.PayoutSubpolicyType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.PayoutLRType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.Lsqlfunction));
            collosysEnumList.AddToList(typeof (ColloSysEnums.OutputType));
            collosysEnumList.AddToList(typeof (ColloSysEnums.CityCategory));

        }
    }
}