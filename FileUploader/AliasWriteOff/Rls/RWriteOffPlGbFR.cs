﻿#region ref

using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploaderService.RecordManager;

#endregion

namespace ColloSys.FileUploaderService.AliasWriteOff.Rls
{
// ReSharper disable once InconsistentNaming
    public class RWriteOffPlGbFR:RWriteOffSharedFR
    {
        public RWriteOffPlGbFR(FileScheduler fileScheduler)
            : base(fileScheduler, new RWriteOffPlGbRC())
        {
            
        }
    }
}
