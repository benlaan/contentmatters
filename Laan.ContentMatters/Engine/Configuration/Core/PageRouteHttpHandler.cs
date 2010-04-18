using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

using Castle.MicroKernel;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Controllers;
using Laan.ContentMatters.Loaders;
using Laan.Persistence.Interfaces;
using System.Web.Mvc;

namespace Laan.ContentMatters.Engine
{
    public class EmptyHandler : IHttpHandler
    {
    	public EmptyHandler( RequestContext requestContext, IKernel kernel )
    	{
    		
    	}
    
        #region IHttpHandler Members

        public bool  IsReusable
        {
	        get { return true; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
 	        
        }

        #endregion
    }

    internal class PageRouteHttpHandler : MvcHandler // EmptyHandler
    {
        private IKernel _kernel;
        private RequestContext _requestContext;

        public PageRouteHttpHandler( RequestContext requestContext, IKernel kernel ) : base( requestContext )
        {
            _requestContext = requestContext;
            _kernel = kernel;
        }

        #region IHttpHandler Members

        protected override void ProcessRequest( HttpContext context )
        {
            IMapper mapper = _kernel.Resolve<IMapper>();

            var siteProperties = new SiteProperties( new Dictionary<string, object>() );
            PageLoader loader = new PageLoader( mapper, siteProperties );
            SitePage page = loader.GetPageFromPath( context.Request.Path );

            string action = page.Action;
            string controllerName = page.GetType().Name;

            RouteValueDictionary routeValues = _requestContext.RouteData.Values;
            routeValues.Add( "controller", controllerName );
            routeValues.Add( "action", action );
            routeValues.Add( "key", page.Key );

            routeValues.Add( "page", page );
            routeValues.Add( "site", loader.Site );

            var controller = _kernel.Resolve<IPageController>();
            controller.Execute( _requestContext );
        }

        //public bool IsReusable
        //{
        //    get { return true; }
        //}

        #endregion
    }
}
