using System;
using System.Diagnostics;
using System.Collections.Generic;

using Castle.Core.Logging;
using Castle.Facilities.NHibernateIntegration;

using NHibernate;
using NHibernate.Cfg;

namespace MatrixGroup.Data.Persistence
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ILogger _logger;
        private readonly ISessionManager _manager;

        private Dictionary<string, IUnitOfWork> _units;

        public UnitOfWorkFactory( ILogger logger, ISessionManager manager )
        {
            _logger = logger;
            _manager = manager;
            _units = new Dictionary<string, IUnitOfWork>();
        }

        private bool UnitOfWorkNotDefined(string alias)
        {
            string key = GetKeyFromThread();

            if (!_units.ContainsKey(key))
                return true;

            if (_units[key].RefCount == 0)
                return true;

            if (!_units[key].Session.IsOpen)
                return true;

            return false;
        }

        private void CreateUnitOfWork()
        {
            var key = GetKeyFromThread();

            _manager.DefaultFlushMode = FlushMode.Never;
            _units[key] = new UnitOfWork(_logger, _manager.OpenSession(), this);
        }

        public IUnitOfWork UnitOfWork()
        {
            string key = GetKeyFromThread();

            if (UnitOfWorkNotDefined())
                CreateUnitOfWork();
            else
                _units[key].RefCount++;

            return _units[key];
        }

        public ISession OpenSession()
        {
            if (UnitOfWorkNotDefined())
                CreateUnitOfWork();

            return _units[GetKeyFromThread()].Session;
        }

        public void RemoveUnit()
        {
            string key = GetKeyFromThread();

            if (_units.ContainsKey(key) && _units[key].RefCount != 0)
                throw new Exception("RefCount must be zero before removing unit");

            _units.Remove(key);
        }

        public System.Data.IDbCommand CreateCommand()
        {
            string key = GetKeyFromThread();
            ISession session = OpenSession();
            System.Data.IDbCommand command = session.Connection.CreateCommand();

            if (_units.ContainsKey(key))
                _units[key].Enlist(command);

            return command;
        }

        private string GetKeyFromThread()
        {
            return string.Format( "{1}", System.Threading.Thread.CurrentThread.ManagedThreadId );
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach ( KeyValuePair<string, IUnitOfWork> unitOfWork in _units )
            {
                ISession session = unitOfWork.Value.Session;
                if ( session != null && session.IsOpen )
                    session.Close();
            }
            _units.Clear();
        }

        #endregion
    }
}
