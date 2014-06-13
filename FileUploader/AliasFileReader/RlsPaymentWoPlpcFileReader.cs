using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploader.AliasFileReader
{
   public class RlsPaymentWoPlpcFileReader: FileReader<Payment>
    {
       public RlsPaymentWoPlpcFileReader(FileScheduler file)
            : base(new RlsPaymentWoPlpcRecordCreator(file))
        {
            
        }
    }
}
