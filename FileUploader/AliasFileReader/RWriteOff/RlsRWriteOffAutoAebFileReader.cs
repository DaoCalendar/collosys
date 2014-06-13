using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
    public class RlsRWriteOffAutoAebFileReader:FileReader<RWriteoff>
    {
        public RlsRWriteOffAutoAebFileReader(FileScheduler fileScheduler)
            : base(new RlsRRWriteOffAutoAebrecordCreator(fileScheduler))
        {
        }
    }
}
