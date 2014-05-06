using System;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Enumerations;
using ColloSys.FileUploader.AliasFileReader;
using ColloSys.FileUploadService.Implementers;
using FileUploaderService.Interfaces;
using NLog;

namespace FileUploaderService
{
    public static class AllFileUploader
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        public static void UploadFile(FileScheduler scheduler)
        {
            IDbLayer dbLayer=new DbLayer();
            switch (scheduler.FileDetail.AliasName)
            {
                #region RlsPayment

                case ColloSysEnums.FileAliasName.R_PAYMENT_LINER:
                    var paymentLiner = new RlsPaymentLinerFileReader(scheduler);
                    paymentLiner.ReadAndSaveBatch();
                     _log.Info(string.Format("BatchProcessing : PostProcessing Start"));
            scheduler.UploadStatus = ColloSysEnums.UploadStatus.PostProcessing;
            dbLayer.ChangeStatus(scheduler);
            //ReaderNeeds.PostProcesing();
            _log.Info(string.Format("BatchProcessing : PostProcessing() Done"));

                    _log.Info("ReadFile: Retry error record.");
            //ReaderNeeds.RetryErrorRows();

            _log.Info("ReadFile: saving the error table.");
                    dbLayer.SetDoneStatus(scheduler,);
            SaveDoneStatus();

                    break;

                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_AEB:
                    var paymentWo = new RlsPaymentWoAebFileReader(scheduler);
                    paymentWo.ReadAndSaveBatch();
                    break;

                case ColloSysEnums.FileAliasName.R_MANUAL_REVERSAL:
                    var paymentManual = new RlsPaymentManualReversalFileReader(scheduler);
                    paymentManual.ReadAndSaveBatch();
                    break;

                case ColloSysEnums.FileAliasName.R_PAYMENT_WO_PLPC:
                    var paymentWoplpc = new RlsPaymentWoPlpcFileReader(scheduler);
                    paymentWoplpc.ReadAndSaveBatch();
                    break;

                #endregion

                case ColloSysEnums.FileAliasName.E_PAYMENT_LINER:
                    var ebbspaymentLiner = new EbbsPaymentLinerFileReader(scheduler);
                    ebbspaymentLiner.ReadAndSaveBatch();
                    break;

                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_AUTO:
                    var ePaymentWo = new EbbsPaymentWoAutoFileReader(scheduler);
                    ePaymentWo.ReadAndSaveBatch();
                    break;

                case ColloSysEnums.FileAliasName.E_PAYMENT_WO_SMC:
                    var ePaymentEoSmc = new EbbsPaymentWoSmcFileReader(scheduler);
                    ePaymentEoSmc.ReadAndSaveBatch();
                    break;

                #region

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
                    throw new NotImplementedException("File Reader not implemented for given type");
            }
        }
        
    }
}
