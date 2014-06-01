using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasRecordCreator
{
    class DhflRecodCreator : AliasDHFLrecordCreator
    {
        private const uint AccountPosition=2;
        private const uint AccountLength = 8;
        private readonly IList<int> _paymentcodes;

        public DhflRecodCreator(FileScheduler fileScheduler)
            : base(fileScheduler,AccountPosition,AccountLength)
        {
            _paymentcodes = new List<int> { 179, 201, 203, 891 };
        }

        public override bool GetComputations(DHFL_Liner record, IExcelReader reader)
        {
            //record.IsDebit = _paymentcodes.Contains(record.TransCode);
           
            //var amountdiff = record.DebitAmount - (record.CreditAmount.HasValue ? record.CreditAmount.Value : 0);
            //record.TransAmount = amountdiff * (amountdiff < 0 ? -1 : 1);
            return true;
        }
    }
}
