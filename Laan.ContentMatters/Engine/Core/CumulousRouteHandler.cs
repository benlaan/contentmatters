using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Castle.MicroKernel;

namespace Laan.ContentMatters.Engine
{
    public abstract class CumulousRouteHandler : IRouteHandler
    {

        public CumulousRouteHandler()
        {
        }

        #region IRouteHandler Members

        public abstract IHttpHandler GetHttpHandler( RequestContext requestContext );

        #endregion

        protected static IHttpHandler GetHandlerForVersion( RequestContext requestContext )
        {
            IKernel kernel = IoC.Container.Kernel;
            Version version = typeof( MvcHandler ).Assembly.GetName().Version;
            // ASP.NET MVC 2 doesn't like overriding MvcHandler
            if ( version.Major == 2 )
                return new CumulousHandler( requestContext, kernel );
            else
                return new CumulousMvcHandler( requestContext, kernel );
        }
    }
}
