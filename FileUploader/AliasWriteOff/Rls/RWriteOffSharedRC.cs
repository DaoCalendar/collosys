#region ref

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffSharedRC:RecordCreator<RWriteoff>
    {
        public override bool ComputedSetter(RWriteoff entity)
        {
            throw new NotImplementedException();
        }

        public override bool IsRecordValid(RWriteoff entity)
        {
            throw new NotImplementedException();
        }

        public override bool CheckBasicField()
        {
            throw new NotImplementedException();
        }

        public override RWriteoff GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
