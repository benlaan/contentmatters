using System;
using System.Collections.Generic;

using Castle.Core.Logging;

using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

using Laan.Persistence.Interfaces;

namespace Laan.Persistence
{
    public class RepositoryWithType<T, I> : IRepositoryWithType<T, I>, IDisposable where T : class
    {
        protected readonly ILogger _logger;
        protected readonly ISessionFactory _factory;

        public RepositoryWithType( ILogger logger, ISessionFactory factory )
        {
            _factory = factory;
            _logger = logger;
        }

        [ThreadStatic]
        private ISession _session;

        protected virtual ISession Session
        {
            get
            {
                if ( _session == null || !_session.IsConnected || !_session.IsOpen )
                    _session = _factory.OpenSession();
                return _session;
            }
            private set
            {
                _session = value;
            }
        }

        protected virtual INHibernateQueryable<T> LinqSession()
        {
            ILinq<T> linq = new Linq<T>( Session );
            return linq.Query();
        }
        
        private void ClearSession()
        {
            if ( StatelessSession )
                Session.Clear();
        }

        public virtual T Get( I id )
        {
            ClearSession();
            return Session.Get<T>( id );
        }

        public virtual IList<T> GetAll()
        {
            ClearSession();
            ICriteria criteria = Session.CreateCriteria<T>();
            return criteria.List<T>();
        }

        public virtual IList<T> FindAll( IDictionary<string, object> propertyValuePairs )
        {
            ClearSession();
            ICriteria criteria = Session.CreateCriteria<T>();

            foreach ( string key in propertyValuePairs.Keys )
                criteria.Add( Expression.Eq( key, propertyValuePairs[ key ] ) );

            return criteria.List<T>();
        }

        public virtual IList<T> FindAllByAssociation( string associate, I id )
        {
            ClearSession();
            ICriteria criteria = Session.CreateCriteria<T>();
            criteria.CreateCriteria( associate ).Add( Expression.Eq( "ID", id ) );

            return criteria.List<T>();
        }

        public virtual T FindOne( IDictionary<string, object> propertyValuePairs )
        {
            IList<T> foundList = FindAll( propertyValuePairs );

            if ( foundList.Count > 1 )
                throw new NonUniqueResultException( foundList.Count );
            else
                if ( foundList.Count == 1 )
                    return foundList[ 0 ];

            return default( T );
        }

        public virtual void Delete( T entity )
        {
            Session.Delete( entity );
            Session.Flush();
            ClearSession();
        }

        public virtual T SaveOrUpdate( T entity )
        {
            Session.SaveOrUpdate( entity );
            Session.Flush();
            ClearSession();
            return entity;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Session.Close();
        }

        #endregion

        public bool StatelessSession { get; set; }
    }
}
