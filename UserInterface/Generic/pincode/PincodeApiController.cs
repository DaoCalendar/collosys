#region references

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AngularUI.Shared.apis;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.SharedDomain;
using ColloSys.QueryBuilder.ClientDataBuilder;
using ColloSys.QueryBuilder.GenericBuilder;
using ColloSys.UserInterface.Areas.Generic.ViewModels;
using ColloSys.UserInterface.Shared;
using ColloSys.UserInterface.Shared.Attributes;
using NHibernate.Linq;
using ColloSys.DataLayer.Enumerations;

#endregion


namespace ColloSys.UserInterface.Areas.Generic.apiController
{
    public class PincodeApiController : BaseApiController<GPincode>
    {
        private static readonly GPincodeBuilder GPincodeBuilder=new GPincodeBuilder();
        private static readonly InfoBuilder InfoBuilder=new InfoBuilder();

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetStates()
        {
            var data = GPincodeBuilder.StateList();
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPincodes(string state)
        {
            var data = GPincodeBuilder.OnState(state);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPincodesArea(string area, string city)
        {
            var data = GPincodeBuilder.AreaOnCityAndArea(area, city);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [HttpTransaction]
        public IEnumerable<string> GetMissingPincodes(string pincode)
        {
            var missingPincodes = new List<string>();
            missingPincodes.AddRange(InfoBuilder.MissingPincodeId(pincode));
            return missingPincodes.Take(10).ToList();
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetPincodeCity(string city, string district)
        {
            var data = GPincodeBuilder.CitiesOnCityDistrict(city, district);
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
                var regiondata = GPincodeBuilder.GetOnExpression(x => x.Country == pincodedata.Country)
                                                .Select(x => x.Region)
                                                .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, regiondata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.State))
            {
                var statedata =
                    GPincodeBuilder.GetOnExpression(
                        x => x.Country == pincodedata.Country && x.Region == pincodedata.Region)
                                   .Select(x => x.State)
                                   .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, statedata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.Cluster))
            {
                var clusterdata = GPincodeBuilder.GetOnExpression(x => x.Country == pincodedata.Country && x.Region == pincodedata.Region && x.State == pincodedata.State)
                              .Select(x => x.Cluster)
                              .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, clusterdata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.District))
            {
                var districtdata =
                    GPincodeBuilder.GetOnExpression(
                        x =>
                        x.Country == pincodedata.Country && x.Region == pincodedata.Region &&
                        x.State == pincodedata.State && x.Cluster == pincodedata.Cluster)
                                   .Select(x => x.District)
                                   .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, districtdata);
            }

            if (string.IsNullOrWhiteSpace(pincodedata.City))
            {
                var citydata =
                    GPincodeBuilder.GetOnExpression(
                        x =>
                        x.Country == pincodedata.Country && x.Region == pincodedata.Region &&
                        x.State == pincodedata.State && x.Cluster == pincodedata.Cluster &&
                        x.District == pincodedata.District)
                                   .Select(x => x.City)
                                   .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, citydata);
            }
            return null;
        }

        [HttpGet]
        [HttpTransaction]
        public HttpResponseMessage GetWholePincode()
        {
            var data = GPincodeBuilder.GetAll().Select(x => x.Pincode).ToList();
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