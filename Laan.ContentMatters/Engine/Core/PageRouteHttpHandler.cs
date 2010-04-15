using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Castle.MicroKernel;

using Laan.ContentMatters.Controllers;
using Laan.Persistence.Interfaces;
using Laan.ContentMatters.Configuration;
using System.Collections.Generic;
using Laan.ContentMatters.Loaders;

namespace Laan.ContentMatters.Engine
{
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

            var siteProperties = new SiteProperties(new Dictionary<string, object>());
            PageLoader loader = new PageLoader( mapper, siteProperties );
            SitePage page = loader.GetPageFromPath( context.Request.Path );

            // RouteData Values
            string action = page.Action;
            string controllerName = page.GetType().Name;

            RouteValueDictionary routeValues = RequestContext.RouteData.Values;
            routeValues.Add( "controller", controllerName );
            routeValues.Add( "action", action );
            routeValues.Add( "key", page.Key );

            routeValues.Add( "page", page );
            routeValues.Add( "site", loader.Site );

            var controller = _kernel.Resolve<IPageController>();
            controller.Execute( RequestContext );
        }

        #endregion
    }
}
