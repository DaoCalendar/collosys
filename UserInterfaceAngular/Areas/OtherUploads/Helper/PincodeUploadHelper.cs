#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.FileUploadService.Excel2DT;
using ColloSys.Shared.ExcelWriter;
using ColloSys.Shared.Types4Product;
using NLog;

#endregion

namespace ColloSys.UserInterface.Areas.OtherUploads.Helper
{
    public static class PincodeUploadHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool IsFileValid(HttpPostedFileBase fileBase, out string message)
        {
            if (fileBase == null || fileBase.ContentLength <= 0)
            {
                message = "Empty file!!!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(fileBase.FileName))
            {
                message = "File name empty!!!";
                return false;
            }

            var fileInfo = new FileInfo(fileBase.FileName);
            if (fileInfo.Extension != ".xlsx")
            {
                message = "Not Excel(.xlsx) file!!!";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static void ReadPincodeExcel(ScbEnums.Products product, HttpPostedFileBase fileBase)
        {
            DataTable dataTable;
            try
            {
                dataTable = EpPlusExcelsxReader.ReadExcelData(typeof(PincodeRow), fileBase.InputStream);
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error: could not read excel.", e);
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
                Logger.ErrorException("Error: could not get RECInfo from db.", e);
                throw new Exception("Could not fetch customer info from db.", e);
            }

            UpdatePincodes(sharedInfoList, dataTable);
        }

        private static void UpdatePincodes(IList listdata, DataTable dataTable)
        {
            var enumerator = listdata.GetEnumerator();
            if (!enumerator.MoveNext()) { return; }
            dynamic firstItem = enumerator.Current;
            dynamic typedEnumerable = ExcelWriter.ConvertTyped(listdata, firstItem);
            UploadPincodesTyped(typedEnumerable, dataTable);
        }

        private static void UploadPincodesTyped<TInfo>(IEnumerable<TInfo> infoList, DataTable dataTable)
            where TInfo : CustomerInfo
        {
            ISet<TInfo> infoSet = new HashSet<TInfo>(infoList);
            var nhSession = SessionManager.GetCurrentSession();
            foreach (DataRow row in dataTable.Rows)
            {
                var pincoderow = new PincodeRow(row[0].ToString(), row[1].ToString(), row[2].ToString());
                if (!pincoderow.IsRecordvalid) continue;
                var record = infoSet.FirstOrDefault(x => x.AccountNo == pincoderow.AccountNo);
                if (record == null) continue;
                if (record.Pincode == pincoderow.Pincode) continue;
                record.Pincode = pincoderow.Pincode;
                nhSession.SaveOrUpdate(record);
            }
        }
    }
}