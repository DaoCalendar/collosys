using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.UserInterface.Areas.Generic.ViewModels;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;
using ColloSys.DataLayer.Enumerations;

namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class PincodeApiController : BaseApiController<GPincode>
    {
        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStates()
        {
            var data = Session.QueryOver<GPincode>().Select(x => x.State).List<string>().Select(x => x.ToUpper()).Distinct();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPincodes(string state)
        {
            var data = Session.QueryOver<GPincode>().Where(x => x.State == state).List();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPincodesArea(string area, string city)
        {
            var data = Session.Query<GPincode>()
                          .Where(x => x.Area.ToString().StartsWith(area) && x.City == city)
                          .Take(10).Select(x => x.Area)
                          .ToList()
                          .Distinct();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public List<string> GetMissingPincodes(string pincode)
        {
            var missingPincodes = new List<string>();

            // credit card pin code
           
            missingPincodes.AddRange(Session.Query<Info>()
                                    .Where(x => x.GPincode == null && x.Pincode.ToString().StartsWith(pincode))
                                    .Select(x => x.Pincode.ToString())
                                    .Distinct()
                                    .ToList());

            
            return missingPincodes.Take(10).ToList();
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPincodeCity(string city, string district)
        {
            var data = Session.Query<GPincode>()
                          .Where(x => x.City.ToString().StartsWith(city) && x.District == district)
                          .Take(10).Select(x => x.City)
                          .ToList()
                          .Distinct();
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> GetCityCategory()
        {
           return Enum.GetNames(typeof (ColloSysEnums.CityCategory));
        }

        [HttpPost]
        [HttpTransaction]
        public HttpResponseMessage GetWholedata(PincodeData pincodedata)
        {
            if (pincodedata == null)
                return null;

            if (string.IsNullOrWhiteSpace(pincodedata.Region))
            {
                var regiondata = Session.QueryOver<GPincode>()
                              .Where(x => x.Country == pincodedata.Country)
                              .Select(x => x.Region)
                              .List<string>();
                return Request.CreateResponse(HttpStatusCode.OK, regiondata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.State))
            {
                var statedata = Session.QueryOver<GPincode>()
                               .Where(x => x.Country == pincodedata.Country && x.Region == pincodedata.Region)
                               .Select(x => x.State)
                               .List<string>();
                return Request.CreateResponse(HttpStatusCode.OK, statedata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.Cluster))
            {
                var clusterdata = Session.QueryOver<GPincode>()
                              .Where(x => x.Country == pincodedata.Country && x.Region == pincodedata.Region && x.State == pincodedata.State)
                              .Select(x => x.Cluster)
                              .List<string>();
                return Request.CreateResponse(HttpStatusCode.OK, clusterdata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.District))
            {
                var districtdata = Session.QueryOver<GPincode>()
                              .Where(x => x.Country == pincodedata.Country && x.Region == pincodedata.Region && x.State == pincodedata.State && x.Cluster == pincodedata.Cluster)
                              .Select(x => x.District)
                              .List<string>();
                return Request.CreateResponse(HttpStatusCode.OK, districtdata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.City))
            {
                var citydata = Session.QueryOver<GPincode>()
                              .Where(x => x.Country == pincodedata.Country && x.Region == pincodedata.Region && x.State == pincodedata.State && x.Cluster == pincodedata.Cluster && x.District == pincodedata.District)
                              .Select(x => x.City)
                              .List<string>();
                return Request.CreateResponse(HttpStatusCode.OK, citydata);
            }

            return null;

        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetWholePincode()
        {
            var data = Session.QueryOver<GPincode>().Select(x => x.Pincode).List<uint>();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

    }
}

//missingPincodes.AddRange(Session.Query<RInfo>()
//                       .Where(x => x.GPincode == null && x.Pincode.ToString().StartsWith(pincode))
//                       .Select(x => x.Pincode.ToString())
//                       .Distinct()
//                       .ToList());
//missingPincodes.AddRange(Session.Query<EInfo>()
//                       .Where(x => x.GPincode == null && x.Pincode.ToString().StartsWith(pincode))
//                       .Select(x => x.Pincode.ToString())
//                       .Distinct()
//                       .ToList());