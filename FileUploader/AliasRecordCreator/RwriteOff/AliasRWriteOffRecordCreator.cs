using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploader.RowCounter;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploaderService.AliasRecordCreator.RwriteOff
{
   public abstract  class AliasRWriteOffRecordCreator:IAliasRecordCreator<RWriteoff>
   {
       public FileScheduler FileScheduler { get; private set; }

       

       public AliasRWriteOffRecordCreator(FileScheduler fileScheduler)
       {

       }

       public bool ComputedSetter(RWriteoff obj, IExcelReader reader, ICounter counter)
       {
           throw new NotImplementedException();
       }

       public bool ComputedSetter(RWriteoff obj, RWriteoff yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
       {
           throw new NotImplementedException();
       }

       public bool CheckBasicField(IExcelReader reader, ICounter counter)
       {
           throw new NotImplementedException();
       }

       public bool IsRecordValid(RWriteoff record, ICounter counter)
       {
           throw new NotImplementedException();
       }
   }
}
