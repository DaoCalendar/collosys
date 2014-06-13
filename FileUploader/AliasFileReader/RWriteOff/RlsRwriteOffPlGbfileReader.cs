using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
    public class RlsRwriteOffPlGbfileReader : FileReader<RWriteoff>
    {
        public RlsRwriteOffPlGbfileReader(FileScheduler fileScheduler)
            : base(new RlsRWriteOffPlGbRecordCreator(fileScheduler))
        {

        }
    }
}
