#region references

using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasPayment
{
    public abstract class PaymentRecordCreator : RecordCreator<Payment>
    {
        #region ctor
        private readonly uint _accountPosition;
        private readonly uint _accountLength;

        protected PaymentRecordCreator(uint accountPosition, uint accountLength)
        {
            _accountLength = accountLength;
            _accountPosition = accountPosition;
        }
        #endregion

        #region abstract implemntations
        public override bool ComputedSetter(Payment obj)
        {
            try
            {
                obj.FileDate = FileScheduler.FileDate.Date;
                obj.AccountNo = ulong.Parse(Reader.GetValue(_accountPosition))
                    .ToString("D" + _accountLength.ToString(CultureInfo.InvariantCulture));
                GetComputations(obj);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool CheckBasicField()
        {
            // loan no should be a number
            ulong loanNumber;
            if (!ulong.TryParse(Reader.GetValue(_accountPosition), out loanNumber))
            {
                //Counter.IncrementIgnoreRecord();
                Log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", Reader.GetValue(_accountPosition)));
                return false;
            }

            // loan number must be of 2 digits min
            return (loanNumber.ToString(CultureInfo.InvariantCulture).Length >= 2);
        }

        public override Payment GetRecordForUpdate()
        {
            return new Payment();
        }

        public virtual bool ComputedSetter(Payment obj, Payment yobj, IEnumerable<FileMapping> mappings)
        {
            return true;
        }

        public override bool IsRecordValid(Payment record)
        {
            return true;
        }
        #endregion

        #region abstract
        protected abstract bool GetComputations(Payment obj);
        public override bool ComputedSetter(Payment entity, Payment preEntity)
        {
            throw new NotImplementedException();
        }

        public override Payment GetPreviousDayEntity(Payment entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
