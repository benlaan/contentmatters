using System;

using Castle.Core;

using NHibernate;
using NHibernate.Linq;

using Laan.Persistence.Interfaces;

namespace Laan.Persistence
{
    /// <summary>
    /// Wrapper for NHibernate.Linq support
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Transient]
    public class Linq<T> : ILinq<T>
    {
        private readonly ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Linq( ISession session )
        {
            _session = session;
        }

        #region ILinq<T> Members

        public INHibernateQueryable<T> Query()
        {
            return _session.Linq<T>();
        }

        #endregion
    }
}