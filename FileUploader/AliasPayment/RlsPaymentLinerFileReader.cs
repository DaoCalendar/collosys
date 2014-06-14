using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasPayment
{
    public class RlsPaymentLinerFileReader : FileReader<Payment>
    {
        public RlsPaymentLinerFileReader(FileScheduler file)
            : base(file, new RlsPaymentLinerRecordCreator())
        {
            
        }
    }
}
