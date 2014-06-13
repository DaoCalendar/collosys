using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
    public  class RlsRWriteOffAutoGbFileReader:FileReader<RWriteoff>
    {
        public RlsRWriteOffAutoGbFileReader(FileScheduler fileScheduler)
            : base(new RlsRWriteOffAutoGbRecordCreator(fileScheduler))
        {
        }
    }
}
