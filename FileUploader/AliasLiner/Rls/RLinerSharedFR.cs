#region refernces

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner.Rls
{
// ReSharper disable once InconsistentNaming
   public class RLinerSharedFR:FileReader<RLiner>
   {
       public RLinerSharedFR(FileScheduler fileScheduler, IRecord<RLiner> recordCreator) 
           : base(fileScheduler, recordCreator)
       {
       }

       public override bool PostProcessing()
       {
           throw new System.NotImplementedException();
       }
   }
}
