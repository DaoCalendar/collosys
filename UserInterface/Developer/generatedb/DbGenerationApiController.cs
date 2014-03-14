#region references

using System.Collections.Generic;
using System.Web.Http;
using ColloSys.DataLayer.Enumerations;
using ColloSys.UserInterface.Areas.Developer.Models;
using ColloSys.UserInterface.Shared.Attributes;
using UserInterfaceAngular.Areas.Generic.Models;
#endregion


namespace ColloSys.UserInterface.Areas.Developer.apiController
{
    public class DbGenerationApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public void CreateDatabase()
        {
            CreateDb.CreateDatabse();

        }

        [HttpGet]
        public IEnumerable<string> GetSectionsNames()
        {
            var data = ConnectionStringMgr.GetSectionsNames();
            return data;
        }

        [HttpPost]
        public void EncryptData()
        {
            const string sectionName = "connectionStrings";
            Cryptography.EncryptConnString(sectionName);
        }

        [HttpPost]
        public void DecryptData()
        {
            const string sectionName = "connectionStrings";
            Cryptography.DecryptConnString(sectionName);
        }

        [HttpGet]
        public void EncryptSection(string sectionName)
        {
            Cryptography.EncryptConnString(sectionName);
        }

        [HttpGet]
        public void DecryptSection(string sectionName)
        {
            Cryptography.DecryptConnString(sectionName);
        }
    }
}






//[System.Web.Mvc.HttpPost]
//[UserActivity(Activity = ColloSysEnums.Activities.Development)]
//public CreateDb CreateDatabase()
//{
//    CreateDb.CreateDatabse();
//    return RedirectToAction("SignOut");
//}

