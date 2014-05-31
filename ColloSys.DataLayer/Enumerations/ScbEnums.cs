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
            SMC,
            HL,
            NHL,
            ALL
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
            Payment,
            RWriteoff,
            ELiner,
            //EPayment,
            EWriteoff,
            CLiner,
            CUnbilled,
            //CPayment,
            CWriteoff,
            CacsActivity,
            Info,
            DHFL_Liner
            //RInfo,
            //CInfo,
            //EInfo
        }

    }
}
