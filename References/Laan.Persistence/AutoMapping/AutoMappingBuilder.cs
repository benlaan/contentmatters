using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

using Castle.Core.Configuration;
using Castle.Facilities.NHibernateIntegration;
using Castle.Facilities.NHibernateIntegration.Builders;
using Castle.MicroKernel;

using FluentNHibernate.AutoMap;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using Laan.Persistence.Interfaces;

namespace Laan.Persistence.AutoMapping
{
    public class AutoMappingBuilder : IConfigurationBuilder
    {
        private IConfiguration _facility;
        private IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the AutoMappingBuilder class.
        /// </summary>
        public AutoMappingBuilder( IKernel kernel )
        {
            _kernel = kernel;
        }

        public Configuration GetConfiguration( IConfiguration facility )
        {
            _facility = facility;

            var configuration = new Configuration();
            ApplyConfigurationSettings( configuration, _facility.Children[ "settings" ] );

            AutoPersistenceModel auto = BuildAutoMappings();

            //var pathMapper = _kernel.Resolve<IMapper>();
            var path = configuration.Properties[ "connection.connection_string" ]; //.Replace( "~", pathMapper.MapPath( "~" ) );
            var database = SQLiteConfiguration.Standard.ConnectionString( c => c.Is( path ) );

            Fluently
                .Configure( configuration )
                .Database( database )
                .Mappings( m => m.AutoMappings.Add( auto ) )
                .BuildSessionFactory();

            RebuildDatabase( AppDomain.CurrentDomain.BaseDirectory, configuration );

            return configuration;
        }

        protected void ApplyConfigurationSettings( Configuration cfg, IConfiguration facilityConfig )
        {
            if ( facilityConfig != null )
            {
                foreach ( IConfiguration configuration in facilityConfig.Children )
                {
                    string name = configuration.Attributes[ "key" ];
                    string value = configuration.Value;
                    cfg.SetProperty( name, value );
                }
            }
        }

        private Assembly ProcessAssemblyName( string assemblyName )
        {
            var probePath = ReadOption( "path" );
            string file = Path.Combine( probePath, assemblyName + ".dll" );
            if (File.Exists( file ) )
                return Assembly.LoadFrom( file );
            else
                return Assembly.Load( assemblyName );
        }

        private AutoPersistenceModel BuildAutoMappings()
        {
            var assemblies = ReadSettings( "assemblies", assemblyName => ProcessAssemblyName( assemblyName ), true );
            var conventions = ReadSettings( "conventions", c => Type.GetType( c, true ) );

            AutoPersistenceModel auto = new AutoPersistenceModel();

            Type filterType = Type.GetType( ReadOption( "filterType", true ) );
            string baseTypeText = ReadOption( "baseType" );
            Type baseType = Type.GetType( baseTypeText, true );
            auto
                .AddEntityAssemblies( assemblies )
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

            auto.CompileMappings();

            if ( OptionExists( "outputMappings" ) )
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Mappings";

                if ( !Directory.Exists( path ) )
                    Directory.CreateDirectory( path );

                auto.WriteMappingsTo( path );
            }

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

        private void RebuildDatabase( string path, Configuration configuration )
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
