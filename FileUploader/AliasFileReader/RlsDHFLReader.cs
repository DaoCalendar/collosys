using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasRecordCreator;
using ColloSys.FileUploader.FileReader;

namespace ColloSys.FileUploader.AliasFileReader
{
    public class RlsDhflReader : FileReader<DHFL_Liner>
    {
        public RlsDhflReader(FileScheduler file)
            : base(new DhflRecodCreator(file))
        {

        }
    }
}
