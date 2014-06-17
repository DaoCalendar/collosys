#region references

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Ebbs
{
// ReSharper disable once InconsistentNaming
    public abstract class EbbsLinerSharedFR:FileReader<ELiner>
    {
        public EbbsLinerSharedFR(FileScheduler fileScheduler, IRecord<ELiner> recordCreator) 
            : base(fileScheduler, recordCreator)
        {
        }
    }
}
