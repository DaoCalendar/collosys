using ColloSys.DataLayer.Enumerations;

namespace ColloSys.Shared.Types
{
    public static class ClassConstant
    {
        public const string CreditCardWriteOff = "CREDITCARDWRITEOFF";
        public const string CreditCardLiner = "CREDITCARDLINER";
        public const string PersonalLoanWriteOff = "PERSONALLOANWRITEOFF";
        public const string PersonalLoanLiner = "PERSONALLOANLINER";

        public static readonly string BFSLiner = CombineProductCategory(ScbEnums.Products.BFS, ScbEnums.Category.Liner);
        public const string BFSWriteOff = "BFSWRITEOFF";
        public const string MortLiner = "MORTLINER";
        public const string MortWriteOff = "MORTWRITEOFF";
        public const string SMELiner = "SMELINER";
        public const string SMEWriteOff = "SMEWRITEOFF";
        public const string PLLiner = "PLLINER";
        public const string PLWriteOff = "PLWRITEOFF";
        public const string CCLiner = "CCLINER";
        public const string CCWriteOff = "CCWRITEOFF";

        public const string AUTOLiner = "AUTOLINER";
        public const string AUTOWriteOff = "AUTOWRITEOFF";

        public static string CombineProductCategory(ScbEnums.Products productName, ScbEnums.Category category)
        {
            return productName.ToString().ToUpper() + category.ToString().ToUpper();
        }
    }
}