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
using Laan.ContentMatters.Loaders;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Controllers;

namespace Laan.ContentMatters.Engine
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException( string path ) : base( String.Format( "No Page found with name '{0}'", path ) ) { }
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
            IMapper mapper = _kernel.Resolve<IMapper>();

            PageLoader loader = new PageLoader( mapper );
            var page = loader.GetPageFromPath( context.Request.Path );

            // RouteData Values
            string action = page.Action;
            //string name = page.Name;
            string controllerName = "Page";

            RouteValueDictionary routeValues = RequestContext.RouteData.Values;
            routeValues.Add( "page", page );
            routeValues.Add( "action", action );
            routeValues.Add( "key", page.Key );
            //routeValues.Add( "name", name );
            routeValues.Add( "controller", controllerName );

            var controller = _kernel.Resolve<IPageController>();
            controller.Execute( RequestContext );
        }

        #endregion
    }
}
