using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

using Laan.ContentMatters.Engine;
using Laan.ContentMatters.Engine.Services;
using Laan.ContentMatters.Engine.Interfaces;

using log4net.Config;

namespace Laan.ContentMatters
{
    public class MvcApplication : HttpApplication
    {
        private IWindsorContainer _container;

        public void RegisterRoutes( RouteCollection routes )
        {
            routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );
            routes.Add( new Route( "{*path}", new Laan.ContentMatters.Engine.DebuggableRouteHandler() ) );
        }

        private void InitialiseWindsor()
        {
            if ( _container != null )
                return;

            IoC.ConfigPath = Server.MapPath( ConfigurationManager.AppSettings[ "castle.config" ] );
            _container = IoC.Container;
            _container.Kernel.Resolver.AddSubResolver(new ArrayResolver(_container.Kernel));

        }

        protected void Application_Start()
        {
            XmlConfigurator.Configure();

            var definitionService = new DefinitionService( new ServerMapper() );
            definitionService.BuildTypesFromDefinitions();

            InitialiseWindsor();

            //Build.TestData();            
            ViewEngines.Engines.Add( _container.Resolve<ICumulousViewEngine>() );

            RegisterRoutes( RouteTable.Routes );
        }
    }
}