using System;
using System.Data;

using NHibernate;

namespace Laan.Persistence.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        string Alias { get; }
        void Enlist( IDbCommand command );
        ISession Session { get; }
        int RefCount { get; set; }
        void Commit();
    }
}
