#region references

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ColloSys.DataLayer.Components;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.FileUploadService.BaseReader;
using ColloSys.FileUploadService.Reader;
using FileUploadService.Reader;

#endregion

namespace ColloSys.FileUploadService.ExcelReader
{
    internal class EPaymentReaderNew //: SingleTableExcelReader<EPayment>
    {
        #region constructor

        private IList<EPayment> _oldRecordList;

        protected DateTime FileDate { get; set; }

        private EPaymentReaderNew(FileScheduler file)
            //: base(file)
        {
            FileDate = file.FileDate;
            SetOldRecordList(FileDate);
        }

        private void SetOldRecordList(DateTime fileDate)
        {
            _oldRecordList = SessionManager.GetCurrentSession().QueryOver<EPayment>()
               .Where(x => x.FileDate == fileDate.Date).List();
        }

        //public static bool ValidateErrorRecord(DataRow errorDataRow, out string errorMessage)
        //{
        //    var fileSchedulerId = Guid.Parse(errorDataRow[UploaderConstants.ErrorFileUploaderId].ToString());

        //    var file = new EPaymentReader(fileSchedulerId);
        //    var record = file.GetTRecord(errorDataRow, out errorMessage);

        //    return (record != null);
        //}

        //public static bool SaveErrorRecord(DataRow errorDataRow, out string errorMessage)
        //{
        //    var fileSchedulerId = Guid.Parse(errorDataRow[UploaderConstants.ErrorFileUploaderId].ToString());

        //    var file = new EPaymentReader(fileSchedulerId);
        //    var record = file.GetTRecord(errorDataRow, out errorMessage);

        //    if (record == null)
        //    {
        //        return false;
        //    }

        //    return file.SaveTRecord(record, out errorMessage);
        //}

        #endregion

        #region Static Methods

        public static void UploadFile(FileScheduler scheduler)
        {
            var file = new EPaymentReader(scheduler);
            file.UploadFile();
        }

        //public static bool UploadEditedErrorRow(FileDetail fileDetail, IList<FileMapping> fileMappings, DataRow dataRow, out string errorMessage)
        //{
        //    //var fileDate = Convert.ToDateTime(dataRow[UploaderConstaints.ErrorFileDate]);
        //    var file = new EPaymentReader(fileDetail, fileMappings, dataRow);
        //    return file.SaveEditedErrorRow(dataRow, out errorMessage);
        //}

        private bool? _isFileValid;

        protected bool IsFileValid()
        {
            if (_isFileValid != null)
                return _isFileValid.Value;

            _isFileValid = true;

            if (!InputFile.Exists)
            {
                ErrorMessage = "File Not Exist";
                _isFileValid = false;
                return _isFileValid.Value;
            }

            if ((ulong)InputFile.Length != FileScheduled.FileSize)
            {
                ErrorMessage = "File Length not valid";
                _isFileValid = false;
                return _isFileValid.Value;
            }

            var expectedExt = "." + Enum.GetName(typeof(EnumHelper.FileType), FileDetail.FileType);
            if (InputFile.Extension.ToUpper() != expectedExt.ToUpper())
            {
                ErrorMessage = "File Extention is not valid";
                _isFileValid = false;
                return _isFileValid.Value;
            }

            return _isFileValid.Value;
        }

        #endregion

        #region overrid methods

        protected override bool PopulateComputedValue(EPayment record, DataRow dr, out string errorDescription)
        {
            var fileMappings = FileMappingList.Where(m => m.ValueType == EnumHelper.FileMappingValueType.ComputedValue
                                                        && m.ActualTable == record.GetType().Name);

            foreach (var mapping in fileMappings)
            {
                try
                {
                    // sub product
                    if (mapping.ActualColumn == record.GetMemberName<EPayment>(x => x.Products))
                    {
                        record.Products = DecodeScbProduct.GetEbbsSubProduct(dr[mapping.TempColumn].ToString());
                        continue;
                    }

                    // isPayment
                    if (mapping.ActualColumn == record.GetMemberName<EPayment>(x => x.IsPayment))
                    {
                        record.IsPayment = (BaseTypeUtilities.ConvertToDecimal(dr[mapping.TempColumn].ToString()) > 0);
                        continue;
                    }

                    // file date
                    if (mapping.ActualColumn == record.GetMemberName<EPayment>(x => x.FileDate))
                    {
                        record.FileDate = FileDate;
                    }

                    // bill date
                    if (mapping.ActualColumn == record.GetMemberName<EPayment>(x => x.BillDate))
                    {
                        record.BillDate = FileDate;
                    }
                }
                catch (Exception ex)
                {
                    //AddInErrorTable(dr, mapping);
                    //errorOnMapping = mapping;
                    errorDescription = GetErrorDescription(mapping, ex.Message);
                    return false;
                }
            }

            //errorOnMapping = null;
            errorDescription = string.Empty;
            return true;
        }

        protected override bool IsRecordValid(EPayment record, out string errorDescription)
        {
            errorDescription = string.Empty;
            return true;
        }

        protected override EPayment GetByUniqueKey(EPayment record)
        {
            return _oldRecordList.SingleOrDefault(x => x.FileDate == FileDate && x.AccountNo == record.AccountNo);
        }

        protected override bool PerformUpdates(EPayment record)
        {
            return false;
        }

        #endregion
    }
}
