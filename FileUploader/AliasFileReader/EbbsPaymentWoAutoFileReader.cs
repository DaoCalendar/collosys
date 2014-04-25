using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploader.Utilities;

namespace ColloSys.FileUploader.AliasFileReader
{
    public class EbbsPaymentWoAutoFileReader : FileReader<Payment>
    {
        public EbbsPaymentWoAutoFileReader(FileScheduler file, List<string> excludeCodes)
            : base(new EbbsPaymentWoAutoRecordCreator(file, excludeCodes))
        {

        }
    }
}
