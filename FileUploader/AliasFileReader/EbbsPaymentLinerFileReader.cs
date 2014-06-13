using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploaderService.AliasRecordCreator;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploader.AliasFileReader
{
   public class EbbsPaymentLinerFileReader : FileReader<Payment>
    {
        public EbbsPaymentLinerFileReader(FileScheduler file)
            : base(new EbbsPaymentLinerRecordCreator(file))
        {

        }
    }
}
