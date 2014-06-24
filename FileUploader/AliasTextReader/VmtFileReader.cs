using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;

namespace ColloSys.FileUploaderService.AliasTextReader
{
    public  class VmtFileReader:FileReader<Payment>
    {
        public VmtFileReader(FileScheduler fileScheduler)
            : base(fileScheduler, new VmtRecordCreator())
        {
        }

        public override bool PostProcessing()
        {
            throw new NotImplementedException();
        }
    }
}
