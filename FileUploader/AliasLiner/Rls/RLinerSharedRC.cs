#region references

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
    public class RLinerSharedRC: RecordCreator<RLiner>
    {
        public override bool ComputedSetter(RLiner entity)
        {
            throw new NotImplementedException();
        }

        public override bool ComputedSetter(RLiner entity, RLiner preEntity)
        {
            throw new NotImplementedException();
        }

        public override bool IsRecordValid(RLiner entity)
        {
            throw new NotImplementedException();
        }

        public override bool CheckBasicField()
        {
            throw new NotImplementedException();
        }

        public override RLiner GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
