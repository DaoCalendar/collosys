#region references

using System;
using ColloSys.AllocationService.AllocationLastCode;
using ColloSys.AllocationService.CacsToLineWriteoff;
using ColloSys.AllocationService.EmailAllocations;
using ColloSys.AllocationService.IgnoreCases;
using ColloSys.AllocationService.MoveAllocations;
using ColloSys.AllocationService.PincodeEntry;
using ColloSys.DataLayer.Enumerations;
using ColloSys.Shared.ConfigSectionReader;
using NLog;
using ColloSys.DataLayer.Infra.SessionMgr;
using System.Configuration;
using ColloSys.DataLayer.NhSetup;

#endregion


namespace ColloSys.AllocationService
{
    public static class StartAllocation
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly ConnectionStringSettings ConnString;

        static StartAllocation()
        {
            try
            {
                ConnString = ColloSysParam.WebParams.ConnectionString;
                Logger.Info(string.Format("Allocation Service: Connection String : {0}", ConnString.ConnectionString));
                SessionManager.InitNhibernate(new NhInitParams
                    {
                        ConnectionString = ConnString,
                        DbType = ConfiguredDbTypes.MsSql,
                        IsWeb = false
                    });
                //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
                SessionManager.BindNewSession();
            }
            catch (Exception ex)
            {
                Logger.Error("FileUploader : " + ex);
            }
        }


        public static void Start()
        {
            Logger.Info("Allocation service started");

            //move data from cacs to liner writeoff
            Logger.Info("Cacs to liner/Writeoff data move started");
            try
            {
                MoveCacsData();
            }
            catch (Exception exception)
            {
                Logger.Info("error in moving data from cacs");
                Logger.Error(exception.Message);
            }

            Logger.Info("Cacs to liner/Writeoff data move ended");

            //add gpincode reference to records in liner/writeoff
            Logger.Info("Pincode to liner/Writeoff data move started");
            try
            {
                MovePincodeLinerWriteoff();
            }
            catch (Exception exception)
            {
                Logger.Info("error in moving pincode data");
                Logger.Error(exception.Message);
            }
            Logger.Info("Pincode to liner/Writeoff data move ended");

            Logger.Info("Move allocations from one stakeholder to another started");
            try
            {
                MoveAllocationsLeaveExitStake.Init();
            }
            catch (Exception exception)
            {
                Logger.Info("error in moving allocations from one to another stakeholder");
                Logger.Error(exception.Message);
            }
            Logger.Info("Move allocations from one stakeholder to another ended");

            //ignore already allocated cases from liner/writeoff
            //Logger.Info("Ignore allocated cases from liner/Writeoff started");
            //try
            //{
            //    IgnoreAllocatedcases();
            //}
            //catch (Exception exception)
            //{
            //    Logger.Info("error in ingnore already allocated cases");
            //    Logger.Error(exception.Message);
            //}

            //Logger.Info("Ignore allocated cases from liner/Writeoff ended");

            //allocation process started here
            Logger.Info("Allocation process for all products started");
            try
            {
                StartAllocationProcess();
            }
            catch (Exception exception)
            {
                Logger.Info("error in allocation process");
                Logger.Error(exception.Message);
            }

            Logger.Info("Allocation process for all products ended");

            //unallocated a/c process started here
            Logger.Info("Unallocated a/c process for all products started");
            try
            {
                EndAllocationProcess();
                Logger.Info("Unallocated a/c process for all products ended");
            }
            catch (Exception exception)
            {
                Logger.Info("error in end allocation process");
                Logger.Error(exception.Message);
            }

            try
            {
                EmailAllocations();
                Logger.Info("Sending Email Process ended");
            }
            catch (Exception exception)
            {
                Logger.Info("error in Send Mail allocation process");
                Logger.Error(exception.Message);
            }
        }

        private static void MoveCacsData()
        {
            MoveDataFromCacs.Init();
        }

        private static void MovePincodeLinerWriteoff()
        {
            LinerWriteoffPincodes.Init();
        }

        private static void IgnoreAllocatedcases()
        {
            IgnoreAllocatedCases.Init();
        }

        private static void StartAllocationProcess()
        {
            foreach (ScbEnums.Products products in Enum.GetValues(typeof(ScbEnums.Products)))
            {
                if (products == ScbEnums.Products.UNKNOWN)
                    continue;
                Logger.Info(string.Format("Allocation process for {0} and liner started", products));
                AllocationLayer.Allocation.StartAllocationProcessV2(products, ScbEnums.Category.Liner);

                Logger.Info(string.Format("Allocation process for {0} and Writeoff started", products));
                //AllocationService.AllocationLayer.Allocation.StartAllocationProcessV2(products, ScbEnums.Category.WriteOff);
            }
        }

        private static void EndAllocationProcess()
        {

            foreach (ScbEnums.Products products in Enum.GetValues(typeof(ScbEnums.Products)))
            {
                if (products == ScbEnums.Products.UNKNOWN)
                    continue;
                Logger.Info(string.Format("UnAllocated a/c process for {0} started", products));
                UnAllocatedCases.Init(products);
            }
        }

        private static void EmailAllocations()
        {
            EmailProductsAllocations.Init();
        }
    }
}
