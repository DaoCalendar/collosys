using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

       private readonly uint _accountPosition;
       private readonly uint _accountLength;
       private readonly uint _cyclestringPos;

       public AliasRWriteOffRecordCreator(FileScheduler fileScheduler,uint accountPosition,uint accountLength,uint cycleString)
       {
           _accountLength = accountLength;
           _cyclestringPos = cycleString;
           _accountPosition = accountPosition;
           FileScheduler = fileScheduler;
       }

       public bool ComputedSetter(RWriteoff obj, IExcelReader reader, ICounter counter)
       {
           obj.FileDate = FileScheduler.FileDate.Date;
           return true;
       }

       public bool ComputedSetter(RWriteoff obj, RWriteoff yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
       {
           throw new NotImplementedException();
       }

       public bool CheckBasicField(IExcelReader reader, ICounter counter)
       { 
           // loan no should be a number
           ulong loanNumber;
           if (!ulong.TryParse(reader.GetValue(_accountPosition), out loanNumber))
           {
               counter.IncrementIgnoreRecord();
               return false;
           }
           var cycleString = reader.GetValue(_cyclestringPos);
           uint cycle;
           if (!uint.TryParse(cycleString,out cycle ))
           {
               return false;
           }

           // loan number must be of 2 digits min
           return (loanNumber.ToString(CultureInfo.InvariantCulture).Length >= 2);
       }

     

       public bool IsRecordValid(RWriteoff record, ICounter counter)
       {
           throw new NotImplementedException();
       }
   }
}
