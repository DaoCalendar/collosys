#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ColloSys.FileUploaderService.DbLayer;
using ColloSys.FileUploaderService.RecordManager;
using ReflectionExtension.ExcelReader;

#endregion

namespace ColloSys.FileUploaderService.AliasPayment
{
    public abstract class PaymentRecordCreator : RecordCreator<Payment>
    {
        #region ctor
        protected readonly IDbLayer DbLayer;
        private readonly uint _accountPosition;
        private readonly uint _accountLength;

        protected PaymentRecordCreator(uint accountPosition, uint accountLength)
        {
            DbLayer = new DbLayer.DbLayer();
            _accountLength = accountLength;
            _accountPosition = accountPosition;
        }
        #endregion

        #region abstract implemntations
        protected override bool ComputedSetter(Payment obj, IExcelReader reader, ICounter counter)
        {
            try
            {
                obj.FileDate = FileScheduler.FileDate.Date;
                obj.AccountNo = ulong.Parse(reader.GetValue(_accountPosition))
                    .ToString("D" + _accountLength.ToString(CultureInfo.InvariantCulture));
                GetComputations(obj, reader);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        protected override bool CheckBasicField(IExcelReader reader, ICounter counter)
        {
            // loan no should be a number
            ulong loanNumber;
            if (!ulong.TryParse(reader.GetValue(_accountPosition), out loanNumber))
            {
                counter.IncrementIgnoreRecord();
                return false;
            }

            // loan number must be of 2 digits min
            return (loanNumber.ToString(CultureInfo.InvariantCulture).Length >= 2);
        }

        public virtual bool ComputedSetter(Payment obj, Payment yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            return true;
        }

        protected override bool IsRecordValid(Payment record, ICounter counter)
        {
            return true;
        }
        #endregion

        #region abstract
        protected abstract bool GetComputations(Payment obj, IExcelReader reader);
        #endregion
    }
}
