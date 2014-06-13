using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader
{
   public class EbbsPaymentLinerFileReader : FileReader<Payment>
    {
        public EbbsPaymentLinerFileReader(FileScheduler file)
            : base(new EbbsPaymentLinerRecordCreator(file))
        {

        }
    }
}
