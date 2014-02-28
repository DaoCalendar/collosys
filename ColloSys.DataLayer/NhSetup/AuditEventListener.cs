#region References

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using ColloSys.DataLayer.BaseEntity;
using NHibernate.Event;
using NHibernate.Persister.Entity;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    [Serializable]
    internal class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener, IPreDeleteEventListener
    {
        private static string CurrentUser
        {
            get
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.User.Identity.IsAuthenticated))
                {
                    return HttpContext.Current.User.Identity.Name;
                }

                var windowsIdentity = WindowsIdentity.GetCurrent();
                return windowsIdentity != null ? windowsIdentity.Name : "anonymous";
            }
        }

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            var audit = @event.Entity as IAuditedEntity;
            if (audit == null)
                return false;

            Set(@event.Persister, @event.DeletedState, "CreatedBy", CurrentUser);
            Set(@event.Persister, @event.DeletedState, "CreatedOn", DateTime.Now);
            Set(@event.Persister, @event.DeletedState, "CreateAction", "Delete");

            audit.CreatedBy = CurrentUser;
            audit.CreatedOn = DateTime.Now;
            audit.CreateAction = "Delete";

            return false;
        }


        public bool OnPreInsert(PreInsertEvent @event)
        {
            var audit = @event.Entity as IAuditedEntity;
            if (audit == null)
                return false;

            Set(@event.Persister, @event.State, "CreatedBy", CurrentUser);
            Set(@event.Persister, @event.State, "CreatedOn", DateTime.Now);
            Set(@event.Persister, @event.State, "CreateAction", "Insert");

            audit.CreatedBy = CurrentUser;
            audit.CreatedOn = DateTime.Now;
            audit.CreateAction = "Insert";

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var audit = @event.Entity as IAuditedEntity;
            if (audit == null)
                return false;

            if (!@event.Session.IsDirtyEntity(@event.Entity))
                return false;

            Set(@event.Persister, @event.State, "CreatedBy", CurrentUser);
            Set(@event.Persister, @event.State, "CreatedOn", DateTime.Now);
            Set(@event.Persister, @event.State, "CreateAction", "Update");

            audit.CreatedBy = CurrentUser;
            audit.CreatedOn = DateTime.Now;
            audit.CreateAction = "Update";

            return false;
        }

        private static void Set(IEntityPersister persister, IList<object> state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}