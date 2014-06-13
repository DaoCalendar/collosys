using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
    public class RlsRWriteOffPlLoardFileReader:FileReader<RWriteoff>
    {
        public RlsRWriteOffPlLoardFileReader(FileScheduler fileScheduler)
            : base(new RlsRWriteOffPlLordsRecordCreator(fileScheduler))
        {
        }
    }
}
