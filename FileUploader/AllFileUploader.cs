#region references

using System;
using System.IO;
using ColloSys.DataLayer.ClientData;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.AliasFileReader;
using ColloSys.FileUploaderService.AliasLiner.Ebbs;
using ColloSys.FileUploaderService.AliasLiner.Rls;
using ColloSys.FileUploaderService.AliasPayment;
using ColloSys.FileUploaderService.AliasTextReader;
using ColloSys.FileUploaderService.AliasWriteOff.Ebbs;
using ColloSys.FileUploaderService.AliasWriteOff.Rls;
using ColloSys.FileUploaderService.FileReader;
using NLog;

#endregion

namespace ColloSys.FileUploaderService
{
    public static class AllFileUploader
    {
        // upload only one file at a time
        private static bool _isUploadInProgress;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void UploadFile(FileScheduler scheduler)
        {
            if (_isUploadInProgress) return;

            try
            {
                Logger.Info("In Uploading file by Alias Switcher");
                _isUploadInProgress = true;
                UploadFileByAliasSwitcher(scheduler);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Could not upload file : " + scheduler.FileName + "error deswcriptions here--", exception);
                throw;
            }
            finally
            {
                _isUploadInProgress = false;
            }

        }

        private static void UploadFileByAliasSwitcher(FileScheduler scheduler)
        {
            switch (scheduler.FileDetail.AliasName)
            {
                #region RlsPayment

                case ColloSysEnums.FileAliasName.R_PAYMENT_LINER:
                    IFileReader<Payment> paymentLiner = new RlsPaymentLinerFileReader(scheduler);
                    paymentLiner.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.C_PAYMENT_LIT:
                    IFileReader<Payment> paymentLit = new LitUitFileReader(scheduler); 
                    paymentLit.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.C_PAYMENT_VISA:
                    IFileReader<Payment> paymentVmt = new VmtFileReader(scheduler);
                    paymentVmt.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB:
                    var paymentWo = new RlsPaymentWoAebFileReader(scheduler);
                    paymentWo.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.R_MANUAL_REVERSAL:
                    var paymentManual = new RlsPaymentManualReversalFileReader(scheduler);
                    paymentManual.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC:
                    var paymentWoplpc = new RlsPaymentWoPlpcFileReader(scheduler);
                    paymentWoplpc.ProcessFile();
                    break;

                #endregion

                #region EbbsPayment
                case ColloSysEnums.FileAliasName.E_PAYMENT_LINER:
                    var ebbspaymentLiner = new EbbsPaymentLinerFileReader(scheduler);
                    ebbspaymentLiner.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_AUTO:
                    var ePaymentWo = new EbbsPaymentWoAutoFileReader(scheduler);
                    ePaymentWo.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_SMC:
                    var ePaymentEoSmc = new EbbsPaymentWoSmcFileReader(scheduler);
                    ePaymentEoSmc.ProcessFile();
                    break;

                    //case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_SCB:
                    //case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_LORDS:
                    //case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_GB:
                    //case ColloSysEnums.FileAliasName.R_WRITEOFF_SME:
                    //case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB:
                    //case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_GB:
                    //case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB:
                    //  var payment  new RlsPaymentLinerFileReader(scheduler);
                    break;

                #endregion

                #region EWriteoff
                case ColloSysEnums.FileAliasName.E_WRITEOFF_AUTO:
                    var eWriteOffAuto = new EWriteOffAutoFR(scheduler);
                    eWriteOffAuto.ProcessFile();
                    break;
                case ColloSysEnums.FileAliasName.E_WRITEOFF_SMC:
                    var eWriteOffSmc = new EWriteOffSmcFR(scheduler);
                    eWriteOffSmc.ProcessFile();
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB:
                    var rWriteOffAutoAeb = new RWriteOffAutoAebFR(scheduler);
                    rWriteOffAutoAeb.ProcessFile();
                    break;
                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_GB:
                    var rWriteOffAutoGb = new RWriteOffAutoGbFR(scheduler);
                    rWriteOffAutoGb.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB:
                    var rWriteOffAutoScb = new RWriteOffAutoScbFR(scheduler);
                    rWriteOffAutoScb.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_AEB:
                    var rWriteOffPlAeb = new RWriteOffPlAebFR(scheduler);
                    rWriteOffPlAeb.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.R_WRITEOFF_SME:
                    var rWriteOffSme = new RWriteOffSmeFR(scheduler);
                    rWriteOffSme.ProcessFile();
                    break;
                #endregion

                #region Liner

                case ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN:
                    var rlinerBfs = new RLinerBfsFR(scheduler);
                    rlinerBfs.ProcessFile();
                    break;
                case ColloSysEnums.FileAliasName.R_LINER_MORT_LOAN:
                    var rlinerMort = new RLinerMortFR(scheduler);
                    rlinerMort.ProcessFile();
                    break;
                case ColloSysEnums.FileAliasName.R_LINER_PL:
                    var rlinerPl = new RLinerPlFR(scheduler);
                    rlinerPl.ProcessFile();
                    break;
                case ColloSysEnums.FileAliasName.E_LINER_AUTO:
                    var elinerAuto = new EbbsLinerAutoFR(scheduler);
                    elinerAuto.ProcessFile();
                    break;
                case ColloSysEnums.FileAliasName.E_LINER_OD_SME:
                    var elinerOdSme = new EbbsLinerOdSmeFR(scheduler);
                    elinerOdSme.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.C_LINER_COLLAGE:
                    IFileReader<CLiner> paymentCollage = new CCollageFileReader(scheduler);
                    paymentCollage.ProcessFile();
                    break;

                case ColloSysEnums.FileAliasName.C_LINER_UNBILLED:
                    IFileReader<CUnbilled> paymentUnBilled = new CUnBilledFileReader(scheduler);
                    paymentUnBilled.ProcessFile();
                    break;
                #endregion


                #region commented

                //case ColloSysEnums.FileAliasName.CACS_ACTIVITY:
                //    new CacsActivityReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.C_LINER_COLLAGE:
                //    new CCollageReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.C_LINER_UNBILLED:
                //    new CUnbilledReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.C_PAYMENT_LIT:
                //case ColloSysEnums.FileAliasName.C_PAYMENT_UIT:
                //    new LitUitReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.C_PAYMENT_VISA:
                //    new VmtReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.C_WRITEOFF:
                //    new CWriteOffReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.E_LINER_AUTO:
                //case ColloSysEnums.FileAliasName.E_LINER_OD_SME:
                //    new ELinerReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.E_PAYMENT_LINER:
                //case ColloSysEnums.FileAliasName.E_PAYMENT_WO_AUTO:
                //case ColloSysEnums.FileAliasName.E_PAYMENT_WO_SMC:
                //    new EPaymentReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.E_WRITEOFF_AUTO:
                //case ColloSysEnums.FileAliasName.E_WRITEOFF_SMC:
                //    new EWriteOffReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.R_LINER_BFS_LOAN:
                //case ColloSysEnums.FileAliasName.R_LINER_MORT_LOAN:
                //case ColloSysEnums.FileAliasName.R_LINER_PL:
                //    new RLinerReader(scheduler).UploadFile();
                //    break;

                //new RPaymentReader(scheduler).UploadFile();
                //    break;
                //case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_AEB:
                //case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_SCB:
                //case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_LORDS:
                //case ColloSysEnums.FileAliasName.R_WRITEOFF_PL_GB:
                //case ColloSysEnums.FileAliasName.R_WRITEOFF_SME:
                //case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_AEB:
                //case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_GB:
                //case ColloSysEnums.FileAliasName.R_WRITEOFF_AUTO_SCB:
                //  var payment  new RlsPaymentLinerFileReader(scheduler);
                //    break;

                #endregion

                default:
                    throw new InvalidDataException("Alias Name is not implemented : " + scheduler.FileDetail.AliasName);
            }
        }

    }
}
