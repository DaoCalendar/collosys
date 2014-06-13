using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
    public class RlsRWriteOffAutoScbFileReader : FileReader<RWriteoff>
    {
        public RlsRWriteOffAutoScbFileReader(FileScheduler fileScheduler)
            : base(new RlsRWriteOffAutoScbRecordCreator(fileScheduler))
        {

        }
    }
}
