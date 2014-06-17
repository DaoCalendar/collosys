#region references

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Ebbs
{
// ReSharper disable once InconsistentNaming
   public class EWriteOffSharedRC:RecordCreator<EWriteoff>
   {
       public override bool ComputedSetter(EWriteoff entity)
       {
           throw new NotImplementedException();
       }

       public override bool ComputedSetter(EWriteoff entity, EWriteoff preEntity)
       {
           throw new NotImplementedException();
       }

       public override bool IsRecordValid(EWriteoff entity)
       {
           throw new NotImplementedException();
       }

       public override bool CheckBasicField()
       {
           throw new NotImplementedException();
       }

       public override EWriteoff GetRecordForUpdate()
       {
           throw new NotImplementedException();
       }

       public override EWriteoff GetPreviousDayEntity(EWriteoff entity)
       {
           throw new NotImplementedException();
       }
   }
}
