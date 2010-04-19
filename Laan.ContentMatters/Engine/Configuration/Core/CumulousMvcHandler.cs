using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Castle.MicroKernel;
using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Controllers;
using Laan.ContentMatters.Loaders;
using Laan.Persistence.Interfaces;

namespace Laan.ContentMatters.Engine
{
    public class CumulousMvcHandler : MvcHandler
    {
        private IHttpHandler _handler;

        public CumulousMvcHandler( RequestContext requestContext, IKernel kernel )
            : base( requestContext )
        {
            _handler = new CumulousHandler( requestContext, kernel );
        }

        #region IHttpHandler Members

        protected override void ProcessRequest( HttpContext context )
        {
            _handler.ProcessRequest( context );
        }

        protected override bool IsReusable
        {
            get { return _handler.IsReusable; }
        }


        #endregion
    }
}
