using System;

using NHibernate.Linq;

namespace Laan.Persistence.Interfaces
{
    /// <summary>
    /// Wrapper for NHibernate.Linq support
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILinq<T>
    {
        INHibernateQueryable<T> Query();
    }
}