using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ColloSys.DataLayer.Domain;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.UserInterface.Shared.Attributes;

namespace ColloSys.UserInterface.Areas.Stakeholder2.api
{
    public class StakeholderEditApiController : ApiController
    {
        [HttpGet]
        [HttpTransaction]
        public Stakeholders GetStakeholderEditMode()
        {
            var stake = SessionManager.GetCurrentSession()
                                      .QueryOver<Stakeholders>()
                                      .Fetch(x => x.Hierarchy).Eager
                                      .Fetch(x => x.StkhRegistrations).Eager
                                      .Fetch(x => x.GAddress).Eager
                                      .Fetch(x => x.StkhPayments).Eager
                                      .Fetch(x => x.StkhWorkings).Eager
                                      .List()
                                      .FirstOrDefault();
                    //foreach (var stkhPayment in stake.StkhPayments)
                    //{
                    //    stkhPayment.StkhWorkings = null;
                    //}
                    //foreach (var stakeAddress in stake.GAddress)
                    //{
                    //    stakeAddress.MakeEmpty2();
                    //}
                    return stake;
                }
    }
}
