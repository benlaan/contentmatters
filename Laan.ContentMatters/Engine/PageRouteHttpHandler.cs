using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Castle.MicroKernel;

using Laan.Persistence.Interfaces;
using Laan.Utilities.Xml;
using Laan.ContentMatters.Configuration;

namespace Laan.ContentMatters.Engine
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException( string path ) : base( "No Page Found at path " + path ) { }
    }

    public class TemplateNotFoundException : Exception
    {
        public TemplateNotFoundException( string name ) : base( "No Template found with name " + name ) { }
    }

    public class LayoutNotFoundException : Exception
    {
        public LayoutNotFoundException( string name ) : base( "No Layout found with name " + name ) { }
    }

    public class PageRouteHttpHandler : MvcHandler
    {
        private IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the PageRouteHandler class.
        /// </summary>
        public PageRouteHttpHandler( RequestContext requestContext, IKernel kernel )
            : base( requestContext )
        {
            _kernel = kernel;
        }

        #region IHttpHandler Members

        protected override void ProcessRequest( HttpContext context )
        {
            string appData = _kernel.Resolve<IMapper>().MapPath( "~/App_Data" );

            var config = XmlPersistence<PageConfiguration>.LoadFromFile( Path.Combine( appData, "Pages.xml" ) );

            string path = RequestContext.HttpContext.Request.Path;
            Trace.WriteLine( "Path: " + path );
            Trace.WriteLine( "RouteData: " + RequestContext.RouteData );

            PageLoader pageLoader = new PageLoader( config );
            Page page = pageLoader.GetPageFromPath( path );

            // RouteData Values
            string action = "Index";
            string name = page.Name;
            string controllerName = page.Template.DataSources.First().Type;

            RequestContext.RouteData.Values.Add( "page", page );
            RequestContext.RouteData.Values.Add( "action", action );
            RequestContext.RouteData.Values.Add( "id", null ); // TODO: remove this when nanme key works correctly
            RequestContext.RouteData.Values.Add( "name", name );
            RequestContext.RouteData.Values.Add( "controller", controllerName );

            var controller = ControllerBuilder.Current.GetControllerFactory().CreateController( RequestContext, controllerName );
            controller.Execute( RequestContext );
        }

        #endregion
    }
}