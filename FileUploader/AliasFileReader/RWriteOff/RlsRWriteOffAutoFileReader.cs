using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
    public class RlsRWriteOffAutoFileReader:FileReader<RWriteoff>
    {
        public RlsRWriteOffAutoFileReader(FileScheduler fileScheduler)
            : base(new RlsRWriteOffAutoRecordCreator(fileScheduler))
        {
        }

    }
}
