using System;
using System.Data;

using NHibernate;

namespace Laan.Persistence.Interfaces
{
    public interface IUnitOfWorkFactory : IDisposable
    {
        IUnitOfWork UnitOfWork( string alias );
        IUnitOfWork UnitOfWork();
        ISession OpenSession();
        IDbCommand CreateCommand();

        void RemoveUnit( string alias );
    }
}
