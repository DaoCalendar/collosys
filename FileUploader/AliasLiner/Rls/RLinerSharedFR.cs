#region refernces

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public abstract class RLinerSharedFR:FileReader<RLiner>
   {
       public RLinerSharedFR(FileScheduler fileScheduler, IExcelRecord<RLiner> recordCreator) 
           : base(fileScheduler, recordCreator)
       {
       }
   }
}
