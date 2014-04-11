using System.Linq;

namespace ColloSys.FileUploader.Utilities
{
    public static class SharedUtility
    {
         public static bool IsDigit(string str)
        {
            return str.All(char.IsDigit);
        }
    }
}
