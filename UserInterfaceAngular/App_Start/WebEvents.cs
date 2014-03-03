using System.Web.Management;
using NLog;
using ColloSys.DataLayer.Infra.SessionMgr;
using ColloSys.DataLayer.Infra.Domain;
using UserInterfaceAngular.App_Start;

namespace ColloSys.UserInterface.App_Start
{

    public class WebEvents : WebEventProvider
    {
       // private readonly Logger logger = LogManager.GetCurrentClassLogger();

        static WebEvents()
        {
            //AppStartInitialization.AppStartInit();
            NHibernateConfig.InitNHibernate();
        }
        
        public override void ProcessEvent(WebBaseEvent raisedEvent  )
        {
            //logger.Info("Application Start");

            WebEventSave(raisedEvent);
        }
      
        public override void Shutdown()
        {
            //logger.Info("Application Shutdown");
        }

        public override void Flush()
        {
        //    logger.Info("Finish event");
        }

        private void WebEventSave(WebBaseEvent raisedEvent)
        {
            if (raisedEvent.EventCode != 1003 && raisedEvent.EventCode != 1004)
            {
                var webEvents = new WebEvent
                {
                    EventTimeUtc = raisedEvent.EventTimeUtc,
                    EventTime = raisedEvent.EventTime,
                    EventSequence = raisedEvent.EventSequence,
                    EventOccurrence = raisedEvent.EventOccurrence,
                    EventCode = raisedEvent.EventCode,
                    EventDetailCode = raisedEvent.EventDetailCode,
                    Message = raisedEvent.Message,
                };
                SessionManager.GetAutomicDao<WebEvent>().SaveOrUpdate(webEvents);
            }
        }

    }

}