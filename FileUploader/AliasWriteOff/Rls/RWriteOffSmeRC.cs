namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffSmeRC:RWriteOffSharedRC
    {
        private const uint AccountPos=1;
        private const uint AccountNoLength=11;
        private const uint CycleString=4;
        public RWriteOffSmeRC()
            : base(AccountPos, AccountNoLength, CycleString)
        {

        }
    }
}
