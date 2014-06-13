using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader
{
    public class RlsPaymentLinerFileReader : FileReader<Payment>
    {
        public RlsPaymentLinerFileReader(FileScheduler file)
            : base(new RlsPaymentLinerRecordCreator(file))
        {
            
        }
    }
}
