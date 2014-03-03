#region references

using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploadService.ExcelReader;
using ColloSys.FileUploadService.TextReader;

#endregion

namespace ColloSys.FileUploadService
{
    public static class AllFileUploader
    {
        public static void UploadFile(FileScheduler scheduler)
        {
            switch (scheduler.FileDetail.AliasName)
            {
                case ColloSysEnums.FileAliasName.CACS_ACTIVITY:
                    new CacsActivityReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.C_LINER_COLLAGE:
                    new CCollageReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.C_LINER_UNBILLED:
                    new CUnbilledReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.C_PAYMENT_LIT:
                case ColloSysEnums.FileAliasName.C_PAYMENT_UIT:
                    new LitUitReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.C_PAYMENT_VISA:
                    new VmtReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.C_WRITEOFF:
                    new CWriteOffReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.E_LINER_AUTO:
                case ColloSysEnums.FileAliasName.E_LINER_OD_SME:
                    new ELinerReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.E_PAYMENT_LINER:
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_AUTO:
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_SMC:
                    new EPaymentReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.E_WRITEOFF_AUTO:
                case ColloSysEnums.FileAliasName.E_WRITEOFF_SMC:
                    new EWriteOffReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN:
                case ColloSysEnums.FileAliasName.R_LINER_MORT_LOAN:
                case ColloSysEnums.FileAliasName.R_LINER_PL:
                    new RLinerReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.R_PAYMENT_LINER:
                case ColloSysEnums.FileAliasName.R_MANUAL_REVERSAL:
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB:
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC:
                    new RPaymentReader(scheduler).UploadFile();
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_AEB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_SCB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_LORDS:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_GB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_SME:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_GB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB:
                    new RWriteOffReader(scheduler).UploadFile();
                    break;
                default:
                    throw new NotImplementedException("File Reader not implemented for given type");
            }
        }
    }
}
