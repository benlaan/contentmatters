using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;

using Castle.Windsor;

using log4net.Config;

using Laan.ContentMatters.Engine;

namespace Laan.ContentMatters
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication, IContainerAccessor
    {
        public static IWindsorContainer Container { get; private set; }

        IWindsorContainer IContainerAccessor.Container
        {
            get { return Container; }
        }

        public static void RegisterRoutes( RouteCollection routes )
        {
            routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

            routes.MapRoute(
                "ViewItem",                                             // Route name
                "{controller}/{id}/{name}",                             // URL with parameters
                new { controller = "Home", action = "View", id = "" }   // Parameter defaults
            );

            routes.MapRoute(
                "NamedDefault",                                         // Route name
                "{controller}/{action}/{id}/{name}",                    // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );
        }

        private void InitialiseWindsor()
        {
            if ( Container != null )
                return;

            IoC.ConfigPath = Server.MapPath( ConfigurationManager.AppSettings[ "castle.config" ] );
            Container = IoC.Container;
        }

        protected void Application_Start()
        {
            XmlConfigurator.Configure();
            InitialiseWindsor();

            Build.TestData();            
            ViewEngines.Engines.Add( new TemplatingViewEngine() );

            ControllerBuilder.Current.SetControllerFactory( new CustomControllerFactory() );
            RegisterRoutes( RouteTable.Routes );
        }
    }
}