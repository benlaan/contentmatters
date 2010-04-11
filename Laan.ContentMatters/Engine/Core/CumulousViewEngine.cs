using System;
using System.Web.Mvc;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Engine.Interfaces;

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
            Page page = (Page) controllerContext.RouteData.Values[ "page" ];
            if ( page == null )
                throw new PageNotFoundException( controllerContext.RequestContext.HttpContext.Request.Path );

            View view = _viewLoader.Load( page );
            return new ViewEngineResult( view, this );
        }

        public void ReleaseView( ControllerContext controllerContext, IView view )
        {
            
        }

        #endregion
    }
}
