using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasPayment
{
   public class RlsPaymentWoAebFileReader : FileReader<Payment>
    {
        public RlsPaymentWoAebFileReader(FileScheduler file)
            : base(file, new RlsPaymentWoAebRecordCreator())
        {
            
        }
    }
}
