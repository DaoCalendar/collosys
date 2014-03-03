using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using NLog;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace ColloSys.UserInterface.App_Start
{
    public class WebApiTimeoutHandling
    {
        public class TimingActionFilter : ActionFilterAttribute
        {
            private const string Key = "__action_duration__";

            public override void OnActionExecuting(HttpActionContext actionContext)
            {
                if (SkipLogging(actionContext))
                {
                    return;
                }

                var stopWatch = new Stopwatch();
                actionContext.Request.Properties[Key] = stopWatch;
                stopWatch.Start();
            }

            public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
            {
                if (!actionExecutedContext.Request.Properties.ContainsKey(Key))
                {
                    return;
                }

                var stopWatch = actionExecutedContext.Request.Properties[Key] as Stopwatch;
                if (stopWatch == null) return;
                stopWatch.Stop();
                var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
                var controller = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
                LogManager.GetCurrentClassLogger().Debug(string.Format("WebApi: Exection of {0}/{1} took {2}",
                                                                       controller, actionName, stopWatch.Elapsed));
            }

            private static bool SkipLogging(HttpActionContext actionContext)
            {
                return actionContext.ActionDescriptor.GetCustomAttributes<NoLogAttribute>().Any() ||
                        actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<NoLogAttribute>().Any();
            }
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
        public class NoLogAttribute : Attribute
        {

        }

        public class SessionTimeoutHandlingFilter : ActionFilterAttribute
        {
            public override void OnActionExecuting(HttpActionContext actionContext)
            {
                if (actionContext.Request.Headers.Authorization == null)
                {
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            } 
        }
    }
}