#region References

using System;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    public static class SessionExtensions
    {
        public static Boolean IsDirtyEntity(this ISession session, Object entity)
        {
            var sessionImpl = session.GetSessionImplementation();
            var persistenceContext = sessionImpl.PersistenceContext;
            var oldEntry = persistenceContext.GetEntry(entity);
            var className = oldEntry.EntityName;
            var persister = sessionImpl.Factory.GetEntityPersister(className);

            var oldState = oldEntry.LoadedState;
            if (oldState == null) return true;

            var currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);
            var dirtyProps = persister.FindDirty(currentState, oldState, entity, sessionImpl);

            return ((dirtyProps != null) && (dirtyProps.Length > 0));
        }
    }
}


//public static Boolean IsDirtyProperty(this ISession session, Object entity, String propertyName)
//{

//    ISessionImplementor sessionImpl = session.GetSessionImplementation();

//    IPersistenceContext persistenceContext = sessionImpl.PersistenceContext;

//    EntityEntry oldEntry = persistenceContext.GetEntry(entity);

//    String className = oldEntry.EntityName;

//    IEntityPersister persister = sessionImpl.Factory.GetEntityPersister(className);

//    //if ((oldEntry == null) && (entity is INHibernateProxy))
//    //{
//    //    INHibernateProxy proxy = entity as INHibernateProxy;
//    //    Object obj = sessionImpl.PersistenceContext.Unproxy(proxy);
//    //    oldEntry = sessionImpl.PersistenceContext.GetEntry(obj);
//    //}

//    Object[] oldState = oldEntry.LoadedState;

//    Object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);

//    Int32[] dirtyProps = persister.FindDirty(currentState, oldState, entity, sessionImpl);

//    Int32 index = Array.IndexOf(persister.PropertyNames, propertyName);

//    Boolean isDirty = (dirtyProps != null) ? (Array.IndexOf(dirtyProps, index) != -1) : false;

//    return (isDirty);
//}

//public static Object GetOriginalEntityProperty(this ISession session, Object entity, String propertyName)
//{
//    ISessionImplementor sessionImpl = session.GetSessionImplementation();

//    IPersistenceContext persistenceContext = sessionImpl.PersistenceContext;

//    EntityEntry oldEntry = persistenceContext.GetEntry(entity);

//    String className = oldEntry.EntityName;

//    IEntityPersister persister = sessionImpl.Factory.GetEntityPersister(className);

//    //if ((oldEntry == null) && (entity is INHibernateProxy))
//    //{

//    //    INHibernateProxy proxy = entity as INHibernateProxy;

//    //    Object obj = sessionImpl.PersistenceContext.Unproxy(proxy);

//    //    oldEntry = sessionImpl.PersistenceContext.GetEntry(obj);

//    //}

//    Object[] oldState = oldEntry.LoadedState;

//    Object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);

//    Int32[] dirtyProps = persister.FindDirty(currentState, oldState, entity, sessionImpl);

//    Int32 index = Array.IndexOf(persister.PropertyNames, propertyName);

//    Boolean isDirty = (dirtyProps != null) ? (Array.IndexOf(dirtyProps, index) != -1) : false;

//    return (isDirty ? oldState[index] : currentState[index]);

//}

