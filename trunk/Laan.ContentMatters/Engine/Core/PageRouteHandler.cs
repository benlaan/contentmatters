using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace Laan.ContentMatters.Engine
{
    public class PageRouteHandler : IRouteHandler
    {
        private IKernel _kernel;

        public PageRouteHandler( IKernel kernel )
        {
            _kernel = kernel;
        }

        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler( RequestContext requestContext )
        {
            return new PageRouteHttpHandler( requestContext, _kernel );
        }

        #endregion
    }
}