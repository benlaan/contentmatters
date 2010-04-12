using System;
using System.Data;

using Laan.Persistence.Interfaces;

using Castle.Core.Logging;

using NHibernate;

namespace MatrixGroup.Data.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool _completed;
        private readonly ILogger _logger;
        private readonly IUnitOfWorkFactory _factory;
        private readonly ITransaction _transaction;

        public UnitOfWork( ILogger logger, ISession session, IUnitOfWorkFactory factory )
        {
            _logger = logger;
            _factory = factory;
            Session = session;
            session.FlushMode = FlushMode.Commit;
            RefCount = 1;

            _completed = false;
            _transaction = Session.BeginTransaction();

            _logger.DebugFormat( "Created for Session {0}", Session.GetHashCode() );
        }

        #region IUnitOfWork Members

        public void Commit()
        {
            _completed = true;
        }

        public void Dispose()
        {
            RefCount--;

            _logger.DebugFormat( 
                "Dispose, Session {0}, RefCount {1}, Total memory: {2:N}", 
                Session.GetHashCode(), RefCount, GC.GetTotalMemory( false ) 
            );

            if ( RefCount == 0 )
            {
                if ( _completed )
                {
                    Session.Flush();
                    _transaction.Commit();
                }
                else
                    _transaction.Rollback();

                Session.Close();

                _factory.RemoveUnit( this.Alias );
            }
        }

        public int RefCount { get; set; }
        public ISession Session { get; private set; }

        public void Enlist( IDbCommand command )
        {
            if ( _transaction != null )
                _transaction.Enlist( command );
        }

        #endregion
    }
}
