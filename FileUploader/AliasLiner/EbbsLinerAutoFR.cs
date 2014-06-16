#region references

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion


namespace ColloSys.FileUploaderService.AliasLiner
{
// ReSharper disable once InconsistentNaming
    public class EbbsLinerAutoFR:EbbsLinerSharedFR
    {
        public EbbsLinerAutoFR(FileScheduler fileScheduler, IRecord<ELiner> recordCreator) 
            : base(fileScheduler, recordCreator)
        {
        }
    }
}
