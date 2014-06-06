#region references
using System;
using System.Collections.Generic;
using ColloSys.DataLayer.BaseEntity;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.SessionMgr
{
    public class NhRepository<T> where T: Entity, new()
    {
        public NhRepository(ISession session)
        {
            _session = session;

        }


        private readonly ISession _session;


        #region one element

        public T Get(Guid id)
        {
            if (!_session.Transaction.IsActive)
            {
                T obj;
                using (var tx = _session.BeginTransaction())
                {
                    obj = _session.Get<T>(id);
                    tx.Commit();
                }
                return obj;
            }

            return null;
        }

        public T Load(Guid id)
        {
            if (!_session.Transaction.IsActive)
            {
                T obj;
                using (var tx = _session.BeginTransaction())
                {
                    obj = _session.Load<T>(id);
                    tx.Commit();
                }
                return obj;
            }

            return null;
        }

        public void SaveOrUpdate(T entity)
        {
            if (_session.Transaction.IsActive) return;
            using (var tx = _session.BeginTransaction())
            {
                _session.SaveOrUpdate(entity);
                tx.Commit();
            }
        }

        public void SaveOrUpdate(IEnumerable<T> entity)
        {
            if (entity == null)
                return;
            if (_session.Transaction.IsActive) return;
            using (var tx = _session.BeginTransaction())
            {
                foreach (var single in entity)
                {
                    if (single.Id == Guid.Empty)
                        _session.SaveOrUpdate(single);
                    else
                        _session.Merge(single);
                }
                tx.Commit();
            }
        }

        public void SaveOrUpdateWithTransiant(IEnumerable<T> entity)
        {
            if (entity == null)
                return;
            if (!_session.Transaction.IsActive)
            {
                using (var tx = _session.BeginTransaction())
                {
                    foreach (var single in entity)
                    {
                        try
                        {
                            _session.SaveOrUpdate(single);
                        }
                        catch (Exception)
                        {
                            _session.Merge(single);
                        }
                    }
                    tx.Commit();
                }
            }
        }

        public void Delete(Guid id)
        {
            if (_session.Transaction.IsActive) return;
            using (var tx = _session.BeginTransaction())
            {
                var entity = _session.Load<T>(id);
                _session.Delete(entity);
                tx.Commit();
            }
        }

        public void Delete(T entity)
        {
            if (!_session.Transaction.IsActive)
            {
                using (var tx = _session.BeginTransaction())
                {
                    _session.Delete(entity);
                    tx.Commit();
                }
            }
        }

        #endregion

        #region group element

        public IEnumerable<T> GetAll()
        {
            if (!_session.Transaction.IsActive)
            {
                using (var tx = _session.BeginTransaction())
                {
                    var obj = _session.QueryOver<T>().List<T>();
                    tx.Commit();
                    return obj;
                }
            }

            return null;
        }

        #endregion
    }
}
