using System;
using System.Data;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.FileUploadService.BaseReader;
using ColloSys.FileUploadService.ExcelReader;
using ColloSys.Shared.ErrorTables;
using NHibernate.Transform;

namespace ColloSys.FileUploadService.BaseReader
{
    public static class SaveEditedErrorRecord
    {
        public static bool ValidateErrorRow(ColloSysEnums.FileAliasName aliasName, DataRow dataRow, out string errorMessage)
        {
            var fileSchedulerId = Guid.Parse(dataRow[UploaderConstants.ErrorFileUploaderId].ToString());
            var fileScheduler = SessionManager.GetCurrentSession()
                                               .QueryOver<FileScheduler>()
                                               .Where(x => x.Id == fileSchedulerId)
                                               .Fetch(x => x.FileDetail).Eager
                                               .TransformUsing(Transformers.DistinctRootEntity)
                                               .SingleOrDefault();

            switch (aliasName)
            {
                case ColloSysEnums.FileAliasName.E_LINER_AUTO:
                case ColloSysEnums.FileAliasName.E_LINER_OD_SME:
                    var file1 = new ELinerReader(fileScheduler);
                    var record1 = file1.GetTRecord(dataRow, out errorMessage);
                    return (record1 != null);
                case ColloSysEnums.FileAliasName.E_WRITEOFF_AUTO:
                case ColloSysEnums.FileAliasName.E_WRITEOFF_SMC:
                    var file3 = new EWriteOffReader(fileScheduler);
                    var record3 = file3.GetTRecord(dataRow, out errorMessage);
                    return (record3 != null);

                case ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN:
                case ColloSysEnums.FileAliasName.R_LINER_MORT_LOAN:
                case ColloSysEnums.FileAliasName.R_LINER_PL:
                    var file4 = new RLinerReader(fileScheduler);
                    var record4 = file4.GetTRecord(dataRow, out errorMessage);
                    return (record4 != null);

                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_AEB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_SCB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_LORDS:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_GB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_SME:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_GB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB:
                    var file6 = new RWriteOffReader(fileScheduler);
                    var record6 = file6.GetTRecord(dataRow, out errorMessage);
                    return (record6 != null);

                case ColloSysEnums.FileAliasName.E_PAYMENT_LINER:
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_AUTO:
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_SMC:
                    var file2 = new EPaymentReader(fileScheduler);
                    var record2 = file2.GetTRecord(dataRow, out errorMessage);
                    return (record2 != null);
                case ColloSysEnums.FileAliasName.R_PAYMENT_LINER:
                case ColloSysEnums.FileAliasName.R_MANUAL_REVERSAL:
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB:
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC:
                    var file5 = new RPaymentReader(fileScheduler);
                    var record5 = file5.GetTRecord(dataRow, out errorMessage);
                    return (record5 != null);

                default:
                    throw new InvalidOperationException("File Alias Name is not valid");
            }
        }


        public static bool SaveErrorRow(ColloSysEnums.FileAliasName aliasName, DataRow dataRow, out string errorMessage)
        {

            var fileSchedulerId = Guid.Parse(dataRow[UploaderConstants.ErrorFileUploaderId].ToString());
            var fileScheduler = SessionManager.GetCurrentSession()
                                               .QueryOver<FileScheduler>()
                                               .Where(x => x.Id == fileSchedulerId)
                                               .Fetch(x => x.FileDetail).Eager
                                               .TransformUsing(Transformers.DistinctRootEntity)
                                               .SingleOrDefault();


            switch (aliasName)
            {
                case ColloSysEnums.FileAliasName.E_LINER_AUTO:
                case ColloSysEnums.FileAliasName.E_LINER_OD_SME:
                    var file1 = new ELinerReader(fileScheduler);
                    var record1 = file1.GetTRecord(dataRow, out errorMessage);
                    return record1 != null && file1.SaveTRecord(record1, out errorMessage);

                case ColloSysEnums.FileAliasName.E_WRITEOFF_AUTO:
                case ColloSysEnums.FileAliasName.E_WRITEOFF_SMC:
                    var file3 = new EWriteOffReader(fileScheduler);
                    var record3 = file3.GetTRecord(dataRow, out errorMessage);
                    return record3 != null && file3.SaveTRecord(record3, out errorMessage);

                case ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN:
                case ColloSysEnums.FileAliasName.R_LINER_MORT_LOAN:
                case ColloSysEnums.FileAliasName.R_LINER_PL:
                    var file4 = new RLinerReader(fileScheduler);
                    var record4 = file4.GetTRecord(dataRow, out errorMessage);
                    return record4 != null && file4.SaveTRecord(record4, out errorMessage);

                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_AEB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_SCB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_LORDS:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_GB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_SME:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_GB:
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB:
                    var file6 = new RWriteOffReader(fileScheduler);
                    var record6 = file6.GetTRecord(dataRow, out errorMessage);
                    return record6 != null && file6.SaveTRecord(record6, out errorMessage);

                case ColloSysEnums.FileAliasName.E_PAYMENT_LINER:
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_AUTO:
                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_SMC:
                    var file2 = new EPaymentReader(fileScheduler);
                    var record2 = file2.GetTRecord(dataRow, out errorMessage);
                    return record2 != null && file2.SaveTRecord(record2, out errorMessage);

                case ColloSysEnums.FileAliasName.R_PAYMENT_LINER:
                case ColloSysEnums.FileAliasName.R_MANUAL_REVERSAL:
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB:
                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC:
                    var file5 = new RPaymentReader(fileScheduler);
                    var record5 = file5.GetTRecord(dataRow, out errorMessage);
                    return record5 != null && file5.SaveTRecord(record5, out errorMessage);

                default:
                    throw new InvalidOperationException("File Alias Name is not valid");
            }
        }
    }
}
