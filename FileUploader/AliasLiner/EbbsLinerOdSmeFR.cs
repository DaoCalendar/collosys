﻿#region references

using ColloSys.DataLayer.Domain;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasLiner
{
// ReSharper disable once InconsistentNaming
   public class EbbsLinerOdSmeFR:EbbsLinerSharedFR
    {
       public EbbsLinerOdSmeFR(FileScheduler fileScheduler, IRecord<ELiner> recordCreator) : base(fileScheduler, recordCreator)
       {
       }
    }
}
