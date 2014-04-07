#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AngularUI.FileUpload.uploadrcode;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Excel2DT;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.Types4Product;

#endregion

namespace AngularUI.FileUpload.uploadpincode
{
    public static class RcodeUploadHelper
    {
        public static int ReadRcodeExcel(ScbEnums.Products product, FileInfo fileBase)
        {
            DataTable dataTable;
            try
            {
                dataTable = EpPlusExcelsxReader.ReadExcelData(typeof(RcodeRow), fileBase);
            }
            catch (Exception e)
            {
                throw new Exception("Could not read excel.", e);
            }

            IList sharedInfoList;
            try
            {
                var session = SessionManager.GetCurrentSession();
                var sharedInfoType = ClassType.GetInfoType(product);
                var criteria = session.CreateCriteria(sharedInfoType);
                sharedInfoList = criteria.List();
            }
            catch (Exception e)
            {
                throw new Exception("Could not fetch customer info from db.", e);
            }

            return UpdateRcodes(sharedInfoList, dataTable);
        }

        private static int UpdateRcodes(IList listdata, DataTable dataTable)
        {
            var enumerator = listdata.GetEnumerator();
            if (!enumerator.MoveNext()) { return 0; }
            dynamic firstItem = enumerator.Current;
            dynamic typedEnumerable = ExcelWriter.ConvertTyped(listdata, firstItem);
           return UploadRcodesTyped(typedEnumerable, dataTable);
        }

        private static int UploadRcodesTyped<TInfo>(IEnumerable<TInfo> infoList, DataTable dataTable)
            where TInfo : CustomerInfo
        {
            var count = 0;
            ISet<TInfo> infoSet = new HashSet<TInfo>(infoList);
            var nhSession = SessionManager.GetCurrentSession();
            using (var tx = nhSession.BeginTransaction())
            {
                nhSession.Clear();
                foreach (DataRow row in dataTable.Rows)
                {
                    var rcoderow = new RcodeRow(row[0].ToString(), row[1].ToString(), row[2].ToString());
                    if (!rcoderow.IsRecordvalid) continue;
                    else
                    {
                        count++;
                    }
                    var record = infoSet.FirstOrDefault(x => x.AccountNo == rcoderow.AccountNo);
                    if (record == null) continue;
                    if (record.CustStatus == rcoderow.Rcode) continue;
                    record.CustStatus = rcoderow.Rcode;
                    nhSession.SaveOrUpdate(record);
                }
                tx.Commit();
                return count;
            }
        }
    }
}