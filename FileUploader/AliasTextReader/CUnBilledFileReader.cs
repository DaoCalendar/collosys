using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

namespace ColloSys.FileUploaderService.AliasTextReader
{
    public class CUnBilledFileReader : FileReader<CUnbilled>
    {
        public CUnBilledFileReader(FileScheduler fileScheduler)
            : base(fileScheduler, new CUnbilledRecordCreator())
        {
        }

        public override bool PostProcessing()
        {
            var recordCreator = RecordCreatorObj as CUnbilledRecordCreator;
            if (recordCreator == null) throw new InvalidProgramException("RecordCreatorObj must be of type CUnbilledRecordCreator");
            var todayLiner = DbLayer.GetDataForDate<CLiner>(FileScheduler.FileDate);

            var recordsToSave = new List<CLiner>();
            foreach (var liner in todayLiner.Where(liner => recordCreator.UnbilledAmount.ContainsKey(liner.AccountNo)))
            {
                liner.UnbilledDue = recordCreator.UnbilledAmount[liner.AccountNo];
                recordsToSave.Add(liner);
            }

            DbLayer.SaveOrUpdateData(recordsToSave);
            return true;
        }
    }
}
