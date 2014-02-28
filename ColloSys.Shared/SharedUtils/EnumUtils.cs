using System;

namespace ColloSys.Shared.SharedUtils
{
    public static class EnumUtils
    {
        public static String ConvertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        public static TEnumType ConverToEnum<TEnumType>(this String enumValue)
        {
            return (TEnumType)Enum.Parse(typeof(TEnumType), enumValue);
        }
    }
}
