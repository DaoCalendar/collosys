using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasRecordCreator;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploader.AliasFileReader
{
    public class EbbsPaymentWoAutoFileReader : FileReader<Payment>
    {
        public EbbsPaymentWoAutoFileReader(FileScheduler file)
            : base(new EbbsPaymentWoAutoRecordCreator(file))
        {

        }
    }
}
