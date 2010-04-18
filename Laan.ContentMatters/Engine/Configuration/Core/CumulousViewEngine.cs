using System;
using System.Web.Mvc;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Engine.Interfaces;
using System.Collections.Generic;

namespace Laan.ContentMatters.Engine
{
    public interface ICumulousViewEngine : IViewEngine { }

    public class CumulousViewEngine : ICumulousViewEngine
    {
        private IViewLoader _viewLoader;

        public CumulousViewEngine( IViewLoader viewLoader )
        {
            _viewLoader = viewLoader;
        }

        #region IViewEngine Members

        public ViewEngineResult FindPartialView( ControllerContext controllerContext, string partialViewName, bool useCache )
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView( ControllerContext controllerContext, string viewName, string masterName, bool useCache )
        {
            SitePage sitePage = ( SitePage )controllerContext.RouteData.Values[ "page" ];
            if ( sitePage == null )
                throw new PageNotFoundException( controllerContext.RequestContext.HttpContext.Request.Path );

            _viewLoader.GenerateData( sitePage, controllerContext.RouteData.Values );
            View view = _viewLoader.Load( sitePage.Page );
            return new ViewEngineResult( view, this );
        }

        public void ReleaseView( ControllerContext controllerContext, IView view )
        {
            
        }

        #endregion
    }
}
