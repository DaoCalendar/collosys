using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasPayment
{
   public class RlsPaymentManualReversalFileReader : FileReader<Payment>
    {
        public RlsPaymentManualReversalFileReader(FileScheduler file)
            : base(file, new RlsPaymentManualReversalRecordCreator())
        {
            
        }
    }
}
