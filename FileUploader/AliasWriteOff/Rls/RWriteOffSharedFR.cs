#region ref

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffSharedFR:FileReader<RWriteoff>
    {
        protected RWriteOffSharedFR(FileScheduler fileScheduler, IExcelRecord<RWriteoff> recordCreator) 
            : base(fileScheduler, recordCreator)
        {
        }

        public override bool PostProcessing()
        {
            try
            {
                RecordCreatorObj.PostProcessing();
            }
            catch (Exception exception)
            {
                Log.Error("In Post Processing of RWriteoff Shared FR: "+ exception.Message);
                return false;
            }
            return true;
        }
    }
}
