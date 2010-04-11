using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;

using Castle.Windsor;

using log4net.Config;

using Laan.ContentMatters.Engine;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Engine.Services;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

//using RouteDebug;

namespace Laan.ContentMatters
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private IWindsorContainer _container;

        public void RegisterRoutes( RouteCollection routes )
        {
            routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

            //routes.MapRoute(
            //    "ViewItem",                                             // Route name
            //    "{controller}/{id}/{name}",                             // URL with parameters
            //    new { controller = "Home", action = "View", id = "" }   // Parameter defaults
            //);

            //routes.MapRoute(
            //    "NamedDefault",                                         // Route name
            //    "{controller}/{action}/{id}/{name}",                    // URL with parameters
            //    new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            //);

            //routes.MapRoute(
            //    "Default",                                              // Route name
            //    "{controller}/{action}/{id}",                           // URL with parameters
            //    new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            //);

            routes.Add( new Route( "{*path}", new Laan.ContentMatters.Engine.DebuggableRouteHandler( /* _container.Kernel */ ) ) );
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
            definitionService.LoadItemDefinitions();

            InitialiseWindsor();

            //Build.TestData();            
            ViewEngines.Engines.Add( _container.Resolve<ICumulousViewEngine>() );

//            ControllerBuilder.Current.SetControllerFactory( new CustomControllerFactory( _container.Kernel ) );
            RegisterRoutes( RouteTable.Routes );
            //RouteDebugger.RewriteRoutesForTesting( RouteTable.Routes );
        }
    }
}