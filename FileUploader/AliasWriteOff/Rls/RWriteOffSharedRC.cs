#region ref

using System;
using System.Globalization;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public abstract class RWriteOffSharedRC:RecordCreator<RWriteoff>
    {
        private uint AccountPos { get; set; }
        private uint AccountNoLength { get; set; }
        private uint CycleString { get; set; }

        public  RWriteOffSharedRC(uint accPos,uint accNolength,uint cyclestring)
        {
            AccountPos = accPos;
            AccountNoLength = accNolength;
            CycleString = cyclestring;
        }

        public override bool ComputedSetter(RWriteoff entity)
        {
            entity.FileDate =FileScheduler.FileDate;
            return true;
        }

        public override bool ComputedSetter(RWriteoff entity, RWriteoff preEntity)
        {
            throw new NotImplementedException();
        }

        public override bool IsRecordValid(RWriteoff entity)
        {
            return true;
        }

        public override bool CheckBasicField()
        {
            // loan no should be a number
            ulong loanNumber;
            if (!ulong.TryParse(Reader.GetValue(AccountPos), out loanNumber))
            {
                Counter.IncrementIgnoreRecord();
                return false;
            }

            var cycleString = Reader.GetValue(CycleString);
            uint cycle;
            if (!uint.TryParse(cycleString, out cycle))
            {
                return false;
            }

            // loan number must be of 2 digits min
            return (loanNumber.ToString(CultureInfo.InvariantCulture).Length >= 2);
        }

        public override RWriteoff GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }

        public override RWriteoff GetPreviousDayEntity(RWriteoff entity)
        {
            throw new NotImplementedException();
        }
    }
}
