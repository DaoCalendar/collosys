using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Mapping;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator
{
   public class EbbsRwriteOffAutoRecordCreator:AliasWriteOffRecordCreator
    {
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;

        public EbbsRwriteOffAutoRecordCreator(FileScheduler fileScheduler)
            : base(fileScheduler, AccountPosition, AccountLength)
        {
        }

        public override bool GetCheckBasicField(IExcelReader reader, ICounter counter)
        {
            throw new NotImplementedException();
        }
    }
}
