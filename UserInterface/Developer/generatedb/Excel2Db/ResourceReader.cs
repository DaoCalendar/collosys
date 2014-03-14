using System;
using System.IO;
using System.Reflection;

namespace ColloSys.UserInterface.Areas.Developer.Models.Excel2Db
{
    internal class ResourceReader
    {
        public static Stream GetEmbeddedResourceAsStream(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resourceName = String.Format("AngularUI.ExcelData.{0}", name);
            return asm.GetManifestResourceStream(resourceName);
        }
    }
}
