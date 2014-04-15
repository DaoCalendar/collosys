﻿using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using NLog;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
    public class RlsPaymentWoPlpc : IAliasRecordCreator<Payment>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly FileScheduler _uploadedFile;

        public RlsPaymentWoPlpc(FileScheduler fileShedular)
        {
            _uploadedFile = fileShedular;
        }

        public bool ComputedSetter(Payment record, IExcelReader reader, ICounter counter)
        {
            try
            {
                record.FileDate = _uploadedFile.FileDate;
                record.IsDebit = (record.TransAmount > 0);
                return true;
            }
            catch (Exception exception)
            {

                throw new Exception("Computted Record is Not Set", exception);
            }

        }

        public bool ComputedSetter(Payment obj, Payment yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
        {
            return true;
        }

        public bool CheckBasicField(IExcelReader reader, ICounter counter)
        {
            string loanNo = reader.GetValue(1);
            ulong loanNumber;
            if (!ulong.TryParse(loanNo, out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 3))
            {
                _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", loanNo));
                return false;
            }
            return true;
        }

        public bool IsRecordValid(Payment record)
        {
            return true;
        }
    }
}
