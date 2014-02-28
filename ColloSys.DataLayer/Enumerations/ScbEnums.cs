using System;

namespace ColloSys.DataLayer.Enumerations
{
    public static class ScbEnums
    {
        [Serializable]
        public enum Products
        {
            UNKNOWN,
            SME_BIL,
            SME_ME,
            SME_LAP,
            MORT,
            AUTO,
            SME_LAP_OD,
            PL,
            CC,
            AUTO_OD,
            SMC
        }

        [Serializable]
        public enum ScbSystems
        {
            CACS,
            CCMS,
            EBBS,
            RLS
        }

        [Serializable]
        public enum Category
        {
            Activity,
            Liner,
            Payment,
            WriteOff
        }

        public enum ClientDataTables
        {
            RLiner,
            RPayment,
            RWriteoff,
            ELiner,
            EPayment,
            EWriteoff,
            CLiner,
            CUnbilled,
            CPayment,
            CWriteoff,
            CacsActivity,
            RInfo,
            CInfo,
            EInfo
        }

    }
}
