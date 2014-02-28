using System;
using System.Security.Principal;

namespace ColloSys.Shared.SharedUtils
{
    internal static class WindowsAuth
    {
        public static string GetLoggedInUserName()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null)
            {
                var name = windowsIdentity.Name;
                var stop = name.IndexOf("\\", StringComparison.Ordinal);
                return (stop > -1) ? name.Substring(stop + 1, name.Length - stop - 1) : string.Empty;
            }

            return string.Empty;
        }

    }
}
