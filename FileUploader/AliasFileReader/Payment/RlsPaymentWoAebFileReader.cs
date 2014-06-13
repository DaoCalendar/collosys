using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploader.AliasFileReader
{
   public class RlsPaymentWoAebFileReader : FileReader<Payment>
    {
        public RlsPaymentWoAebFileReader(FileScheduler file)
            : base(new RlsPaymentWoAebRecordCreator(file))
        {
            
        }
    }
}
