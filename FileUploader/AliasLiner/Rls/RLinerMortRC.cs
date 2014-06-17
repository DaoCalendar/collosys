using System.Globalization;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.ExcelReader;

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public class RLinerMortRC:RLinerSharedRC
    {
        private const uint AccPos = 1;
        private const uint AccountLength = 8;
       public RLinerMortRC() :
           base(AccPos, AccountLength)
       {
           HasMultiDayComputation = false;
       }

       public override bool ComputedSetter(RLiner entity)
       {
           entity.FileDate = FileScheduler.FileDate.Date;
           entity.AccountNo = ulong.Parse(Reader.GetValue(AccPos))
               .ToString("D" + AccountLength.ToString(CultureInfo.InvariantCulture));
           entity.Bucket = GetRLinerBucketNumber(entity.AgeCode);

           entity.Product = DecodeScbProduct.GetRlsMORTProduct(entity.ProductCode);
           return true;
       }

       public override bool ComputedSetter(RLiner entity, RLiner preEntity)
       {
           throw new System.NotImplementedException();
       }
    }
}
