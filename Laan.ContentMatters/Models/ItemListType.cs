using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NHibernate.UserTypes;
using System.Collections;
using NHibernate.Collection.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace Laan.ContentMatters.Models
{
    public class ItemListType<T> : IUserCollectionType where T : IItem
    {

        public ItemListType()
        {

        }

        #region IUserCollectionType Members

        public bool Contains( object collection, object entity )
        {
            return ( (IList<T>) collection ).Contains( (T) entity );
        }

        public IEnumerable GetElements( object collection )
        {
            return (IEnumerable) collection;
        }

        public object IndexOf( object collection, object entity )
        {
            return ( (IList<T>) collection ).IndexOf( (T) entity );
        }

        public IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
        {
            return new ItemList<T>( session );
        }

        public object Instantiate()
        {
            return new ItemList<T>();
        }

        /// <summary>
        /// Instantiate an empty instance of the "underlying" collection (not a wrapper),
        /// but with the given anticipated size (i.e. accounting for initial size
        /// and perhaps load factor).
        /// </summary>
        /// <param name="anticipatedSize">
        /// The anticipated size of the instantiated collection
        /// after we are done populating it.  Note, may be negative to indicate that
        /// we not yet know anything about the anticipated size (i.e., when initializing
        /// from a result set row by row).
        /// </param>
        public object Instantiate( int anticipatedSize )
        {
            return new ItemList<T>();
        }

        public object ReplaceElements( object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session )
        {
            IList<T> result = (IList<T>) target;
            result.Clear();
            foreach ( object item in (IEnumerable) original )
                result.Add( (T) item );
            return result;
        }

        public IPersistentCollection Wrap( ISessionImplementor session, object collection )
        {
            return new ItemList<T>( session, (ItemList<T>) collection );
        }

        #endregion
    }
}
