using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
    public abstract class AliasPaymentRecordCreator : IAliasRecordCreator<Payment>
    {
        private readonly FileScheduler _scheduler;
        private readonly uint _accountPosition;
        private readonly uint _accountLength;

        protected AliasPaymentRecordCreator(FileScheduler scheduler, uint accountPosition, uint accountLength)
        {
            _scheduler = scheduler;
            _accountLength = accountLength;
            _accountPosition = accountPosition;
        }

        public bool ComputedSetter(Payment obj, IExcelReader reader, ICounter counter)
        {
            try
            {
                obj.FileDate = _scheduler.FileDate.Date;
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

        public abstract bool GetComputations(Payment obj, IExcelReader reader);

        public bool CheckBasicField(IExcelReader reader, ICounter counter)
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

        public virtual bool IsRecordValid(Payment record,ICounter counter)
        {
            return true;
        }
    }
}
