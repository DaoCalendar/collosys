#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Excel2DT;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.Types4Product;

#endregion

namespace AngularUI.FileUpload.uploadpincode
{
    public static class PincodeUploadHelper
    {
        public static int ReadPincodeExcel(ScbEnums.Products product, FileInfo fileBase)
        {
            DataTable dataTable;
            try
            {
                dataTable = EpPlusExcelsxReader.ReadExcelData(typeof(PincodeRow), fileBase);
            }
            catch (Exception e)
            {
                throw new Exception("Could not read excel.", e);
            }

            IList sharedInfoList;
            try
            {
                var session = SessionManager.GetCurrentSession();
                var criteria = session.CreateCriteria(typeof(CustomerInfo));
                sharedInfoList = criteria.List();
            }
            catch (Exception e)
            {
                throw new Exception("Could not fetch customer info from db.", e);
            }

            return UpdatePincodes(sharedInfoList, dataTable);
        }

        private static int UpdatePincodes(IList listdata, DataTable dataTable)
        {
            var enumerator = listdata.GetEnumerator();
            if (!enumerator.MoveNext()) { return 0; }
            dynamic firstItem = enumerator.Current;
            dynamic typedEnumerable = ExcelWriter.ConvertTyped(listdata, firstItem);
            return UploadPincodesTyped(typedEnumerable, dataTable);
        }

        private static int UploadPincodesTyped<TInfo>(IEnumerable<TInfo> infoList, DataTable dataTable)
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
                    var pincoderow = new PincodeRow(row[0].ToString(), row[1].ToString(), row[2].ToString());
                    if (!pincoderow.IsRecordvalid) continue;
                    else
                    {
                        count++;
                    }
                    //increase the counter
                    var record = infoSet.FirstOrDefault(x => x.AccountNo == pincoderow.AccountNo);
                    if (record == null) continue;
                    if (record.Pincode == pincoderow.Pincode) continue;
                    record.Pincode = pincoderow.Pincode;
                    nhSession.SaveOrUpdate(record);
                }
                tx.Commit();
                return count;
            }
        }
    }
}