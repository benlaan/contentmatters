using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Laan.Persistence.Interfaces;
using System;
using Castle.Core.Logging;

namespace Laan.Persistence
{
    /// <summary>
    /// Since nearly all of the domain objects you create will have a type of int ID, this 
    /// most freqently used base GenericDao leverages this assumption.  If you want a persistent 
    /// object with a type other than int, such as string, then use 
    /// <see cref="GenericDaoWithTypedId{T, IdT}" />.
    /// </summary>
    public class Repository<T> : RepositoryWithType<T, int>, IRepository<T> where T : class
    {
        public Repository( ILogger logger, ISessionFactory factory ) : base( logger, factory )
        {
            StatelessSession = false;
        }

        public Repository( ILogger logger, ISessionFactory factory, bool statelessSession ) : base( logger, factory )
        {
            StatelessSession = statelessSession;
        }
    }
}
