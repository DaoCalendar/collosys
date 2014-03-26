using System.Collections.Generic;
using System.Web.Http;

namespace AngularUI.Generic.connection
{
    public class ConnectionApiController : ApiController
    {
        [HttpGet]
        public IEnumerable<ConnectionStringData> GetAllConnectionStrings()
        {
            return ConnectionStringMgr.GetAllConnectionStrings();
        }

        [HttpPost]
        
        public ConnectionStringData Save(ConnectionStringData connection)
        {
            return ConnectionStringMgr.Save(connection);
        }

        [HttpPost]
        
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
