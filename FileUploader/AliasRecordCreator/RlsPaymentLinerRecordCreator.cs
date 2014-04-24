#region references

using System.Collections.Generic;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

#endregion


namespace ColloSys.FileUploader.AliasReader
{
    public class RlsPaymentLinerRecordCreator : AliasPaymentRecordCreator
    {
        #region ctor

        private const uint AccountPosition=3;
        private const uint AccountLength = 8;

        private readonly IList<int> _paymentcodes;


        public RlsPaymentLinerRecordCreator(FileScheduler fileScheduler):base(fileScheduler,AccountPosition,AccountLength)
        {
           _paymentcodes = new List<int> { 179, 201, 203, 891 };
        }

        #endregion

        //public bool ComputedSetter(Payment record, IExcelReader reader, ICounter counter)
        //{
        //    try
        //    {
        //        record.FileDate = _uploadedFile.FileDate;
        //        record.IsDebit = _paymentcodes.Contains(record.TransCode);
        //        var amountdiff = record.DebitAmount - (record.CreditAmount.HasValue ? record.CreditAmount.Value : 0);
        //        record.TransAmount = amountdiff * (amountdiff < 0 ? -1 : 1);
        //        record.AccountNo = ulong.Parse(reader.GetValue(3)).ToString("D8");
        //        return true;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("Computted value has not set", exception);
        //    }
        //}

        //public bool CheckBasicField(IExcelReader reader, ICounter counter)
        //{
        //    var loanNo = reader.GetValue(3);

        //    ulong loanNumber;
        //    if (!ulong.TryParse(loanNo, out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 3))
        //    {
        //        _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", loanNo));
        //        counter.IncrementIgnoreRecord();
        //        return false;
        //    }

        //    return true;
        //}

        //#region Empty Implementor

        //public bool IsRecordValid(Payment record)
        //{
        //    return true;
        //}

        //public bool ComputedSetter(Payment obj, Payment yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        //{
        //    return true;
        //}

        //#endregion

        //public override bool ComputedSetter(Payment record, IExcelReader reader, ICounter counter)
        //{
        //    try
        //    {
        //        record.FileDate = _uploadedFile.FileDate;

        //        record.IsDebit = _paymentcodes.Contains(record.TransCode);

        //        var amountdiff = record.DebitAmount - (record.CreditAmount.HasValue ? record.CreditAmount.Value : 0);
        //        record.TransAmount = amountdiff * (amountdiff < 0 ? -1 : 1);
               
        //        record.AccountNo = ulong.Parse(reader.GetValue(3)).ToString("D8");
        //        return true;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception("Computted value has not set", exception);
        //    }
        //}

        public override bool GetComputations(Payment record, IExcelReader reader)
        {
            record.IsDebit = _paymentcodes.Contains(record.TransCode);

            var amountdiff = record.DebitAmount - (record.CreditAmount.HasValue ? record.CreditAmount.Value : 0);
            record.TransAmount = amountdiff * (amountdiff < 0 ? -1 : 1);
            return true;
        }

        //public override bool CheckBasicField(IExcelReader reader, ICounter counter)
        //{
        //    var loanNo = reader.GetValue(3);

        //    ulong loanNumber;
        //    if (!ulong.TryParse(loanNo, out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 3))
        //    {
        //        _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", loanNo));
        //        counter.IncrementIgnoreRecord();
        //        return false;
        //    }

        //    return true;
        //}
    }
}
