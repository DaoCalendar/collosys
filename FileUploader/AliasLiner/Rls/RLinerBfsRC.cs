using System.Globalization;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.ExcelReader;

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public class RLinerBfsRC:RLinerSharedRC
    {
        private const uint AccPos = 1;
        private const uint AccountLength = 8;
       public RLinerBfsRC() 
           : base(AccPos, AccountLength)
       {
           HasMultiDayComputation = false;
       }

       public override bool ComputedSetter(RLiner entity)
       {
           entity.FileDate = FileScheduler.FileDate.Date;
           entity.AccountNo = ulong.Parse(Reader.GetValue(AccPos))
               .ToString("D" + AccountLength.ToString(CultureInfo.InvariantCulture));
           entity.Bucket = GetRLinerBucketNumber(entity.AgeCode);

           entity.Product = DecodeScbProduct.GetRlsBFSProduct(entity.ProductCode);
           return true;
       }

       public override bool ComputedSetter(RLiner entity, RLiner preEntity)
       {
           return true;
       }
    }
}
