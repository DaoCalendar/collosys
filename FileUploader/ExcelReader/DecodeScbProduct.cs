#region references

using System.Linq;
using ColloSys.DataLayer.Enumerations;

#endregion


namespace ColloSys.FileUploaderService.ExcelReader
{
    public static class DecodeScbProduct
    {
        public static ScbEnums.Products GetEBBSProduct(string value)
        {
            switch (value)
            {
                case "SMARTCREDIT":
                case "SMART CREDIT OVERDRAFT":
                case "0F5":
                case "0F7":
                case "0O1":
                case "0O2":
                    return ScbEnums.Products.SMC;
                case "AEB AL":
                case "Mileage":
                case "AUTO OD":
                case "0F0":
                case "0F6":
                    return ScbEnums.Products.AUTO_OD;
                default:
                    return ScbEnums.Products.UNKNOWN;
            }
        }

        public static ScbEnums.Products GetRlsMORTProduct(uint productCode)
        {
            uint[] smeLapCodes =
            {
                7570, 8200, 8300, 9033, 9034, 9042, 9043, 9045, 9047, 9074, 9075, 9078, 9100, 9102,
                9103
            };

            if (smeLapCodes.Contains(productCode))
            {
                return ScbEnums.Products.SME_LAP;
            }

            return ScbEnums.Products.MORT;
        }

        public static ScbEnums.Products GetRlsBFSProduct(uint productCode)
        {
            // 1200 to 1299 -> SME BIL
            if (productCode >= 1200 && productCode <= 1299)
            {
                return ScbEnums.Products.SME_BIL;
            }

            //1010-05-80-35-15-25-90-00-20-43-40-30-45-41 -> ME
            uint[] meProductCodes = { 1010, 1005, 1080, 1035, 1015, 1025, 1090, 1000, 1020, 1043, 1040, 1030, 1045, 1041 };

            if (meProductCodes.Contains(productCode))
            {
                return ScbEnums.Products.SME_ME;
            }

            return ScbEnums.Products.SME_LAP;
        }

        //public static ScbEnums.Products GetProduct(string value)
        //{
        //    switch (value)
        //    {
        //        case "SMARTCREDIT":
        //        case "SMART CREDIT OVERDRAFT":
        //        case "0F5":
        //        case "0F7":
        //        case "0O1":
        //        case "0O2":
        //            return ScbEnums.Products.SMC;
        //        case "AEB AL":
        //        case "Mileage":
        //        case "AUTO OD":
        //        case "0F0":
        //        case "0F6":
        //            return ScbEnums.Products.AUTO;
        //        case "SME":
        //        case "SME BIL":
        //        case "BIL-ADVANCE EMI":
        //        case "BIL-ADVANCE EMI CP":
        //        case "LACP":
        //        case "LARP":
        //        case "SBL LFP RES PROP":
        //        case "SME BIL DRP":
        //        case "SME GURNTD INSTLN":
        //        case "SMEBILCSHPFTMPR":
        //        case "SME LAP COMM":
        //        case "SME LAP RESIDENTIAL":
        //        case "SMELAPGROSSPROFIT":
        //        case "SMEGTDINSTLOANS18MTH":
        //        case "SMELAPRESEBITDA":
        //        case "SMEBILPRQLDTOLAP":
        //        case "0BS":
        //        case "0BU":
        //        case "SME GIL":
        //        case "SME LFP":
        //        case "SME BIL DRP >90":
        //        case "SMELAPCOMEBITDA":
        //            return ScbEnums.Products.SME;
        //        case "MORT":
        //        case "MOA":
        //        case "HOME LOANS":
        //        case "HOME LOAN":
        //        case "PLOT LOANS":
        //        case "HOME LOANS - NRO":
        //        case "GBMORT":
        //        case "LIC STAFF":
        //        case "LORDS HL-B":
        //        case "LORDS LAP-B":
        //        case "LORDS LAP-N":
        //        case "Mort":
        //        case "HOME SAVER":
        //        case "LAP-RES-NR":
        //            return ScbEnums.Products.MORT;
        //        case "BFS":
        //        case "SME LAP":
        //            return ScbEnums.Products.BFS;
        //        case "EXPRESS":
        //        case "PL BIL":
        //        case "PL":
        //        case "PL TEST":
        //        case "PLRemedial":
        //        case "PROF":
        //        case "AEB PIL LOANS":
        //        case "AEB PL LOANS":
        //        case "AEB PL BIL LOANS":
        //        case "PERSONAL LOAN":
        //        case "LORDS PL-N":
        //        case "LORDS PL-B":
        //        case "AEB-LORDS PL DRP":
        //        case "PERSONAL LOAN - BL":
        //            return ScbEnums.Products.PL;
        //        default:
        //            return ScbEnums.Products.UNKNOWN;
        //    }
        //}
    }
}