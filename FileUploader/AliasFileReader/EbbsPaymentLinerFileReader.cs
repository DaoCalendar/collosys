using System.Collections.Generic;
using System.IO;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.AliasReader;
using ColloSys.FileUploader.FileReader;
using ColloSys.FileUploader.Utilities;

namespace ColloSys.FileUploader.AliasFileReader
{
   public class EbbsPaymentLinerFileReader : FileReader<Payment>
    {
        public EbbsPaymentLinerFileReader(FileScheduler file,List<string> excludeCodes)
            : base(new EbbsPaymentLinerRecordCreator(file))
        {

        }
    }
}
