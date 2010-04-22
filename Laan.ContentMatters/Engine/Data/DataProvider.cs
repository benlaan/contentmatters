using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.Persistence;

using NHibernate;

using Castle.Core.Logging;

namespace Laan.ContentMatters.Engine.Data
{
    public class DataProvider : IDataProvider
    {
        private ISessionFactory _factory;
        private ILogger _logger;
        private IDataDictionary _data;
        private IDefinitionService _definitionService;

        public DataProvider(  ILogger logger, ISessionFactory factory, IDataDictionary data, IDefinitionService definitionService )
        {
            _definitionService = definitionService;
            _data = data;
            _logger = logger;
            _factory = factory;
        }

        private object CreateRepository( string type )
        {
            var typeFromDataSource = Type.GetType( type, true, true );
            var genericType = typeof( DataProviderRepository<> ).MakeGenericType( typeFromDataSource );
            
            return Activator.CreateInstance( genericType, _logger, _factory, _definitionService );
        }

        private object ExecuteSelectionMethod( SitePage page, DataSource dataSource, object repository )
        {
            var methodName = String.Format( "Select{0}", dataSource.Select );
            var method = repository.GetType().GetMethod( methodName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod );

            return method.Invoke( repository, new object[] { page, dataSource } );
        }

        private static bool TypeContainsNamespace( string typeName )
        {
            return typeName.Contains( '.' );
        }

        public IDataDictionary Build( SitePage sitePage )
        {
            _data.Clear();
            foreach ( DataSource dataSource in sitePage.Page.DataSources )
            {
                var type = dataSource.Type;
                var select = dataSource.Select;

                switch ( sitePage.Action )
                {
                    case "detail":
                        if ( select != SelectionMode.Key )
                            continue;
                        break;

                    case "index":
                        //if ( !( new[] { SelectionMode.All, SelectionMode.Parent }.All( sm => sm == select ) ) ) // 
                        if ( select != SelectionMode.All && select != SelectionMode.Parent )
                            continue;
                        break;
                }

                if ( !TypeContainsNamespace( type ) )
                    type = String.Format( "Laan.ContentMatters.Models.Custom.{0}, Laan.ContentMatters.Models.Custom", dataSource.Type );

                object repository = CreateRepository( type );                
                object data = ExecuteSelectionMethod( sitePage, dataSource, repository );

                _data.Add( dataSource.Name, data );
            }
            return _data;
        }
    }
}
