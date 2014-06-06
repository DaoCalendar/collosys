#region references

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Generic;
using ColloSys.DataLayer.SessionMgr;
using ColloSys.QueryBuilder.ClientDataBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.UserInterface.Areas.Generic.ViewModels;

#endregion


namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class PincodeApiController : BaseApiController<GPincode>
    {
        private static readonly GPincodeBuilder GPincodeBuilder = new GPincodeBuilder();
        private static readonly InfoBuilder InfoBuilder = new InfoBuilder();

        [HttpGet]
        public HttpResponseMessage GetStates()
        {
            var data = GPincodeBuilder.StateList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetRegions()
        {
            var data = GPincodeBuilder.RegionList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetClusters()
        {
            var data = GPincodeBuilder.ClusterList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetDistricts()
        {
            var data = GPincodeBuilder.DistrictList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetCity()
        {
            var data = GPincodeBuilder.CityList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetPincodes(string state)
        {
            var data = GPincodeBuilder.OnStateGetList(state);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetPincodesArea(string area, string city)
        {
            var data = GPincodeBuilder.AreaOnCityAndArea(area, city);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public IEnumerable<string> GetMissingPincodes(string pincode)
        {
            var missingPincodes = new List<string>();
            missingPincodes.AddRange(InfoBuilder.MissingPincodeId(pincode));
            return missingPincodes.Take(10).ToList();
        }

        [HttpGet]
        public HttpResponseMessage GetPincodeCity(string city, string district)
        {
            var data = GPincodeBuilder.CitiesOnCityDistrict(city, district);
            return Request.CreateResponse(HttpStatusCode.OK, data);

        }

        [HttpGet]
        public IEnumerable<string> GetCityCategory()
        {
            return new[] { "Tier1", "Tier2" };
        }

        [HttpPost]
        public HttpResponseMessage GetWholedata(PincodeData pincodedata)
        {
            if (pincodedata == null)
                return null;

            if (string.IsNullOrWhiteSpace(pincodedata.Region))
            {
                var regiondata = GPincodeBuilder.FilterBy(x => x.Country == pincodedata.Country && x.IsInUse)
                                                .Select(x => x.Region)
                                                .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, regiondata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.State))
            {
                var statedata =
                    GPincodeBuilder.FilterBy(
                        x => x.Country == pincodedata.Country && x.Region == pincodedata.Region && x.IsInUse)
                                   .Select(x => x.State)
                                   .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, statedata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.Cluster))
            {
                var clusterdata = GPincodeBuilder.FilterBy(x => x.Country == pincodedata.Country && x.Region == pincodedata.Region && x.State == pincodedata.State && x.IsInUse)
                              .Select(x => x.Cluster)
                              .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, clusterdata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.District))
            {
                var districtdata =
                    GPincodeBuilder.FilterBy(
                        x =>
                        x.Country == pincodedata.Country && x.Region == pincodedata.Region &&
                        x.State == pincodedata.State && x.Cluster == pincodedata.Cluster
                         && x.IsInUse)
                                   .Select(x => x.District)
                                   .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, districtdata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.City))
            {
                var citydata =
                    GPincodeBuilder.FilterBy(
                        x =>
                        x.Country == pincodedata.Country && x.Region == pincodedata.Region &&
                        x.State == pincodedata.State && x.Cluster == pincodedata.Cluster &&
                        x.District == pincodedata.District && x.IsInUse)
                                   .Select(x => x.City)
                                   .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, citydata);
            }
            return null;
        }

        [HttpGet]
        public HttpResponseMessage GetWholePincode()
        {
            var session = SessionManager.GetCurrentSession();
            var result = session.QueryOver<GPincode>().Select(x => x.Pincode).List<uint>();
            return Request.CreateResponse(HttpStatusCode.OK, result);
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