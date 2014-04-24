using System;
using System.Collections.Generic;
using System.Globalization;
using ColloSys.DataLayer.Domain;
using ReflectionExtension.ExcelReader;

namespace ColloSys.FileUploader.AliasReader
{
   public abstract class AliasWriteOffRecordCreator :IAliasRecordCreator<RWriteoff>
   {
      
       private readonly uint _accountPosition;
       private readonly uint _accountLength;
       public FileScheduler FileScheduler { get; private set; }
       protected AliasWriteOffRecordCreator(FileScheduler scheduler, uint accountPosition, uint accountLength)
       {
           FileScheduler = scheduler;
           _accountLength = accountLength;
           _accountPosition = accountPosition;
       }

       public bool ComputedSetter(RWriteoff record, IExcelReader reader, ICounter counter)
       {
           record.FileDate = FileScheduler.FileDate;
           record.AccountNo = ulong.Parse(reader.GetValue(_accountPosition))
                   .ToString("D" + _accountLength.ToString(CultureInfo.InvariantCulture));
           return true;
       }

       public bool ComputedSetter(RWriteoff obj, RWriteoff yobj, IExcelReader reader, IEnumerable<FileMapping> mapplings)
       {
           return true;
       }

       public bool CheckBasicField(IExcelReader reader, ICounter counter)
       {
           ulong loanNumber;
           if (!ulong.TryParse(reader.GetValue(1), out loanNumber) || (loanNumber.ToString(CultureInfo.InvariantCulture).Length < 3))
           {
              // _log.Debug(string.Format("Data is rejected, Because account No {0} is not valid number", dr[1]));
               return false;
           }
           return true;
       }

       public  abstract bool GetCheckBasicField(IExcelReader reader, ICounter counter);
       

       public bool IsRecordValid(RWriteoff record, ICounter counter)
       {
           throw new NotImplementedException();
       }
   }
}
