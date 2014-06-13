using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasFileReader.RWriteOff
{
    class EbbsRwriteOffAutoFileReader:FileReader<EWriteoff>
    {
        public EbbsRwriteOffAutoFileReader(FileScheduler file)
            : base(new EbbsRwriteOffAutoRecordCreator(file))
        {
        }
    }
}
