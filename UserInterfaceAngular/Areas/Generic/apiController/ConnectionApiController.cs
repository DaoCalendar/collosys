using System.Collections.Generic;
using System.Web.Http;
using ColloSys.UserInterface.Shared.Attributes;
using UserInterfaceAngular.Areas.Generic.Models;

namespace UserInterfaceAngular.Areas.Generic.apiController
{
    public class ConnectionApiController : ApiController
    {
        [HttpGet]
        public IEnumerable<ConnectionStringData> GetAllConnectionStrings()
        {
            return ConnectionStringMgr.GetAllConnectionStrings();
        }

        [HttpPost]
        [HttpTransaction(Persist = true)]
        public ConnectionStringData Save(ConnectionStringData connection)
        {
            return ConnectionStringMgr.Save(connection);
        }

        [HttpPost]
        [HttpTransaction]
        public bool CheckConnection(ConnectionStringData connection)
        {
            return ConnectionStringMgr.CheckConnection(connection);
        }

        //method used for section names in Encrypt page
        [HttpGet]
        public IEnumerable<string> GetSectionsNames()
        {
            var data = ConnectionStringMgr.GetSectionsNames();
            return data;
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
