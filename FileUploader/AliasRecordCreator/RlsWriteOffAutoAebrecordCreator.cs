using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
    public class RlsWriteOffAutoAebrecordCreator : AliasWriteOffRecordCreator
    {

        public RlsWriteOffAutoAebrecordCreator(FileScheduler file)
            : base(file, 1, 8)
        {
        }

        public override bool GetCheckBasicField(IExcelReader reader, ICounter counter)
        {
            var cycleString = reader.GetValue(4);

            uint cycle;
            return uint.TryParse(cycleString, out cycle);
        }

    }
}
