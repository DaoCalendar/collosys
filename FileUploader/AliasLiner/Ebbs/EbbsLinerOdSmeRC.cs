using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.FileUploaderService.AliasLiner.Ebbs
{
// ReSharper disable once InconsistentNaming
    public class EbbsLinerOdSmeRC:EbbsLinerSharedRC
    {
        private const uint AccountNoPosition = 2;
        private const uint AccountNoLength = 11;
        public EbbsLinerOdSmeRC():base(AccountNoPosition,AccountNoLength)
        {
            HasMultiDayComputation = true;
            //var dbLayer = new DbLayer.DbLayer();
            //PreviousDayLiner = dbLayer.GetDataForPreviousDay<ELiner>(ColloSysEnums.FileAliasName.E_LINER_OD_SME,
            //    FileScheduler.FileDate,FileScheduler.FileDetail.FileCount);
        }
        public override bool ComputedSetter(ELiner entity)
        {
            ulong loanNumber;
            ulong.TryParse(Reader.GetValue(2), out loanNumber);
            entity.AccountNo = loanNumber.ToString("D11");

            // cycle
            entity.Cycle = Convert.ToUInt32(entity.CycleDate.Day);

            // CurrentDue
            //record.CurrentDue = record.CurrentBalance;

            // buckets
            entity.Bucket = GetBucketForELiner(entity);

            
            return true;
        }
    }
}
