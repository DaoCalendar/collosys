#region references

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner
{
// ReSharper disable once InconsistentNaming
    public class EbbsLinerSharedRC:RecordCreator<ELiner>
    {
        public override bool ComputedSetter(ELiner entity)
        {
            throw new NotImplementedException();
        }

        public override bool IsRecordValid(ELiner entity)
        {
            throw new NotImplementedException();
        }

        public override bool CheckBasicField()
        {
            throw new NotImplementedException();
        }

        public override ELiner GetRecordForUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
