using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Castle.Core.Configuration;
using Castle.Facilities.NHibernateIntegration;
using Castle.Facilities.NHibernateIntegration.Builders;

using FluentNHibernate.AutoMap;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using Laan.ContentMatters.Models;
using System.Web;
using System.Web.Hosting;

namespace Laan.ContentMatters.Utilities
{
    public class AutoMappingBuilder : IConfigurationBuilder
    {
        private IConfiguration _facility;

        public NHibernate.Cfg.Configuration GetConfiguration(IConfiguration facility)
        {
            _facility = facility;

            var builder = new DefaultConfigurationBuilder();
            var configuration = builder.GetConfiguration(_facility);

            var auto = BuildAutoMappings();

            string path = configuration.Properties["connection.connection_string"].Replace("~", HostingEnvironment.MapPath( "~" ) );
            var database = SQLiteConfiguration.Standard.ConnectionString( c => c.Is(path) );

            Fluently
                .Configure(configuration)
                .Database(database)
                .Mappings(m => m.AutoMappings.Add(auto))
                .BuildSessionFactory();

            RebuildDatabase(AppDomain.CurrentDomain.BaseDirectory, configuration);

            return configuration;
        }

        private AutoPersistenceModel BuildAutoMappings()
        {
            var assemblies = ReadSettings( "assemblies", a => Assembly.Load( a ), true );
            var conventions = ReadSettings( "conventions", c => Type.GetType( c ) );

            var auto = new AutoPersistenceModel();

            foreach ( Assembly assembly in assemblies )
            {
                Type filterType = Type.GetType( ReadOption( "filterType", true ) );
                string baseTypeText = ReadOption( "baseType" );
                Type baseType = Type.GetType( baseTypeText, true );
                auto
                    .AddEntityAssemblies( new[] { assembly } )
                    .Where(
                        type =>
                            !type.IsAbstract &&
                            !type.IsGenericType &&
                            type.IsClass &&
                            filterType.IsAssignableFrom( type )
                    )
                    .WithSetup(
                        s =>
                        {
                            if ( !String.IsNullOrEmpty( baseTypeText ) )
                                s.IsBaseType = type => type == baseType;

                            s.FindIdentity = type => type.Name == "ID";
                        }
                    );

                auto.ConventionDiscovery.Setup( s => { conventions.ForEach( s.Add ); } );
            }

            auto.CompileMappings();

            if ( OptionExists( "outputMappings" ) )
                auto.WriteMappingsTo( AppDomain.CurrentDomain.BaseDirectory + "\\Mappings" );

            return auto;
        }

        #region Utility Methods
        
        private bool OptionExists( string name )
        {
            return ReadOption( name, false ) != "";
        }

        private string ReadOption( string name )
        {
            return ReadOption( name, false );
        }

        private string ReadOption( string name, bool required )
        {
            IConfiguration settings = _facility.Children[ "options" ];
            if ( required && settings == null )
                throw new ArgumentNullException( String.Format( "option {0} not found", name ) );

            IConfiguration option = settings.Children.FirstOrDefault( c => c.Name.ToLower() == name.ToLower() );
            return option != null ? option.Value : "";
        }

        private List<T> ReadSettings<T>( string node, Func<string, T> action )
        {
            return ReadSettings( node, action, false );
        }

        private List<T> ReadSettings<T>( string node, Func<string, T> action, bool required )
        {
            IConfiguration settings = _facility.Children[ node ];
            if ( required && settings == null )
                throw new ArgumentNullException( String.Format( " must define at least 1 {0}", node ) );

            return settings.Children.Select( c => action( c.Value ) ).ToList();
        }

        private void RebuildDatabase(string path, NHibernate.Cfg.Configuration configuration)
        {
            string rebuild = ReadOption( "rebuild" );
            if ( String.IsNullOrEmpty( rebuild ) )
                return;

            var exporter = new SchemaExport( configuration );
            exporter.SetOutputFile( path + "\\Mappings\\export.sql" );
            exporter.Execute( false, true, false );
        }
        #endregion
    }
}
