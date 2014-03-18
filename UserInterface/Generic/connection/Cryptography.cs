using System.Configuration;
using System.Web.Configuration;

namespace AngularUI.Generic.connection
{
    public static class Cryptography
    {
        public static void EncryptConnString(string sectionName)
        {

            //var config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var section = config.GetSection(sectionName);
            if (section.SectionInformation.IsProtected) return;
            section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
            section.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Full);
        }

        public static void DecryptConnString(string sectionName)
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var section = config.GetSection(sectionName);
            if (!section.SectionInformation.IsProtected) return;
            section.SectionInformation.ForceSave = true;
            section.SectionInformation.UnprotectSection();
            config.Save(ConfigurationSaveMode.Full);
        }
    }
}