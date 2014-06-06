using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.FileReader;

namespace ColloSys.FileUploader.AliasFileReader
{
   public class RlsPaymentManualReversalFileReader : FileReader<Payment>
    {
        public RlsPaymentManualReversalFileReader(FileScheduler file)
            : base(new RlsPaymentManualReversalRecordCreator(file))
        {
            
        }
    }
}
