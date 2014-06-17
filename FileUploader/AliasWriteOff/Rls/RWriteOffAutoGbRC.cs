namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffAutoGbRC:RWriteOffSharedRC
    {
        private const uint AccountPosition = 1;
        private const uint AccountLength = 11;
        private const uint CycleString = 4;
        public RWriteOffAutoGbRC() : base(AccountPosition, AccountLength, CycleString)
        {

        }
    }
}
