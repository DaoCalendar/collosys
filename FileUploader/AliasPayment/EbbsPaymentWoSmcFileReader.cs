using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasPayment
{
   public class EbbsPaymentWoSmcFileReader : FileReader<Payment>
    {
        public EbbsPaymentWoSmcFileReader(FileScheduler file)
            : base(file, new EbbsPaymentWoSmcRecordCreator())
        {

        }
    }
}
