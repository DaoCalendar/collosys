using System.Globalization;
using System.IO;
using System.Linq;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public class RLinerPlRC:RLinerSharedRC
   {
       private const uint AccPos = 1;
       private const uint AccountLength = 8;
       public RLinerPlRC()
           : base(AccPos, AccountLength)
       {
           HasMultiDayComputation = true;
       }

       public override bool ComputedSetter(RLiner entity)
       {
           entity.FileDate = FileScheduler.FileDate.Date;
           entity.AccountNo = ulong.Parse(Reader.GetValue(AccPos))
               .ToString("D" + AccountLength.ToString(CultureInfo.InvariantCulture));
           entity.Bucket = GetRLinerBucketNumber(entity.AgeCode);
           return true;
       }

       public override bool ComputedSetter(RLiner entity, RLiner preEntity)
       {
           if (preEntity == null)
           {
               entity.DelqHistoryString = entity.Bucket.ToString("000000000000");
               entity.Flag = ColloSysEnums.DelqFlag.N;
               return true;
           }

           entity.DelqHistoryString = (entity.Cycle == FileScheduler.FileDate.Day)
                                          ? preEntity.DelqHistoryString.Substring(1) + entity.Bucket
                                          : preEntity.DelqHistoryString;

           GetComputetions(entity);

           //for month start liner
           if (YesterdayRecords.First().FileDate.Month != FileScheduler.FileDate.Month)
           {
               entity.Flag = ColloSysEnums.DelqFlag.N;

               return true;
           }

           entity.Flag = ColloSysEnums.DelqFlag.O;
           return true;
       }
   }
}
