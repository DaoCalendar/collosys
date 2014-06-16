#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.FileReader;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner
{
// ReSharper disable once InconsistentNaming
    public class EbbsLinerSharedFR:FileReader<ELiner>
    {
        public EbbsLinerSharedFR(FileScheduler fileScheduler, IRecord<ELiner> recordCreator) 
            : base(fileScheduler, recordCreator)
        {
        }
    }
}
