using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploaderService.AliasPayment;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploader.AliasFileReader
{
   public class RlsPaymentWoPlpcFileReader: FileReader<Payment>
    {
       public RlsPaymentWoPlpcFileReader(FileScheduler file)
            : base(file, new RlsPaymentWoPlpcRecordCreator())
        {
            
        }

       public override bool PostProcessing()
       {
           throw new System.NotImplementedException();
       }
    }
}
