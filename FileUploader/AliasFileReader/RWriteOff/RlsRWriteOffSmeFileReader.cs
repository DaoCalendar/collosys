using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
    public  class RlsRWriteOffSmeFileReader:FileReader<RWriteoff>
    {
        public RlsRWriteOffSmeFileReader(FileScheduler fileScheduler)
            : base(new RlsRWriteOffSmeRecordCreator(fileScheduler))
        {
        }
    }
}
