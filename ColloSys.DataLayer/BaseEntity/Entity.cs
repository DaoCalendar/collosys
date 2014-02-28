#region references

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections;
using Iesi.Collections.Generic;
using NHibernate;

#endregion

namespace ColloSys.DataLayer.BaseEntity
{
    public abstract class Entity : IAuditedEntity
    {
        #region ctor

        // ReSharper disable VirtualMemberNeverOverriden.Global
        // ReSharper disable MemberCanBeProtected.Global
        public virtual Guid Id { get; set; }
        public virtual int Version { get; set; }
        // ReSharper restore MemberCanBeProtected.Global
        // ReSharper restore VirtualMemberNeverOverriden.Global
        public virtual string CreateAction { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime CreatedOn { get; set; }

        protected Entity()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = Guid.Empty;
            Version = 0;

            CreateAction = String.Empty;
            CreatedBy = String.Empty;
            CreatedOn = new DateTime();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            //InitRelationships();
            InitHashSets();
            InitLists();
            InitString();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        #endregion

        #region clone

        public virtual void CloneUniqueProperties(Entity ent)
        {
            Id = ent.Id;
            Version = ent.Version;
            CreateAction = ent.CreateAction;
            CreatedBy = ent.CreatedBy;
            CreatedOn = ent.CreatedOn;
        }

        public virtual void ResetUniqueProperties()
        {
            Id = Guid.Empty;
            Version = 0;
            CreateAction = "Insert";
            CreatedBy = string.Empty;
            CreatedOn = DateTime.Now;
        }
        #endregion

        #region equals

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity);
        }

        private static bool IsTransient(Entity obj)
        {
            return obj != null &&
                   Equals(obj.Id, default(Guid));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        private bool Equals(Entity other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                       otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        #endregion

        #region hashcode

        private int? _oldHashCode;

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            // ReSharper disable BaseObjectGetHashCodeCallInGetHashCode
            if (_oldHashCode.HasValue)
            {
                return _oldHashCode.Value;
            }

            if (IsTransient(this))
            {
                _oldHashCode = base.GetHashCode();
                return _oldHashCode.Value;
            }

            return Id.GetHashCode();
            // ReSharper restore BaseObjectGetHashCodeCallInGetHashCode
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        #endregion

        #region basic methods

        //public abstract void MakeEmpty(bool forceEmpty = false);

        protected virtual void InitString()
        {
            var properties = GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(string) && x.GetSetMethod(true) != null)
                .ToList();

            foreach (var property in properties)
            {
                property.SetValue(this, string.Empty);
            }
        }

        protected virtual void InitHashSets()
        {
            var properties = GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType &&
                            x.PropertyType.GetGenericTypeDefinition() == typeof(Iesi.Collections.Generic.ISet<>))
                .ToList();

            foreach (var property in properties)
            {
                // get T type of ISet
                if (property.PropertyType.GetGenericArguments().Length > 1) continue;
                var listElemType = property.PropertyType.GetGenericArguments()[0];
                if (listElemType == null) continue;

                // create hashedset
                var constructorInfo = typeof(HashedSet<>)
                    .MakeGenericType(listElemType)
                    .GetConstructor(Type.EmptyTypes);

                //construct object
                if (constructorInfo == null) continue;
                var listInstance = (ISet)constructorInfo.Invoke(null);
                property.SetValue(this, listInstance);
            }
        }

        protected virtual void InitLists()
        {
            var properties = GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType &&
                            x.PropertyType.GetGenericTypeDefinition() == typeof(IList<>))
                .ToList();

            foreach (var property in properties)
            {
                // get T type of ISet
                if (property.PropertyType.GetGenericArguments().Length > 1) continue;
                var listElemType = property.PropertyType.GetGenericArguments()[0];
                if (listElemType == null) continue;

                // create hashedset
                var constructorInfo = typeof(List<>)
                    .MakeGenericType(listElemType)
                    .GetConstructor(Type.EmptyTypes);

                //construct object
                if (constructorInfo == null) continue;
                var listInstance = (IList)constructorInfo.Invoke(null);
                property.SetValue(this, listInstance);
            }
        }

        public virtual void MakeEmpty(bool force = false)
        {
            if(Id == Guid.Empty) return;

            var properties = GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType &&
                            ( x.PropertyType.GetGenericTypeDefinition() == typeof (Iesi.Collections.Generic.ISet<>)
                            || x.PropertyType.GetGenericTypeDefinition() == typeof(IList<>)))
                .ToList();

            foreach (var property in properties)
            {
                var value = property.GetValue(this, null);
                if (force || !NHibernateUtil.IsInitialized(value)) 
                    property.SetValue(this, null);
            }

        }

        #endregion
    }
}
