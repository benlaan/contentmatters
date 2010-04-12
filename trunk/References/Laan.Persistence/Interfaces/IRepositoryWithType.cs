using System;
using System.Collections.Generic;

namespace Laan.Persistence.Interfaces
{
    public interface IRepositoryWithType<T, I>
    {
        /// <summary>
        /// Returns null if a row is not found matching the provided ID.
        /// </summary>
        T Get( I id );
        /// <summary>
        /// Returns all of the items of a given type.
        /// </summary>
        IList<T> GetAll();

        /// <summary>
        /// Looks for zero or more instances using the <see cref="IDictionary{string, object}"/> provided.
        /// The key of the collection should be the property name and the value should be
        /// the value of the property to filter by.
        /// </summary>
        IList<T> FindAll( IDictionary<string, object> propertyValuePairs );

        IList<T> FindAllByAssociation( string associate, I id );

        /// <summary>
        /// Looks for a single instance using the property/values provided.
        /// </summary>
        /// <exception cref="NonUniqueResultException" />
        T FindOne( IDictionary<string, object> propertyValuePairs );

        /// <summary>
        /// For entities with automatatically generated IDs, such as identity, SaveOrUpdate may 
        /// be called when saving or updating an entity.
        /// 
        /// Updating also allows you to commit changes to a detached object.  More info may be found at:
        /// http://www.hibernate.org/hib_docs/nhibernate/html_single/#manipulatingdata-updating-detached
        /// </summary>
        T SaveOrUpdate( T entity );

        /// <summary>
        /// I'll let you guess what this does.
        /// </summary>
        void Delete( T entity );
    }
}
