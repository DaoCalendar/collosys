using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploader.AliasFileReader
{
   public class EbbsPaymentWoSmcFileReader : FileReader<Payment>
    {
        public EbbsPaymentWoSmcFileReader(FileScheduler file)
            : base(new EbbsPaymentWoSmcRecordCreator(file))
        {

        }
    }
}
