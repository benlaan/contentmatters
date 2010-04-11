using System;
using System.Web.Mvc;

using Castle.Core.Logging;

namespace Laan.ContentMatters.Controllers
{
    public class PageController : Controller, IPageController
    {
        protected ILogger _log;

        public PageController( ILogger log )
        {
            _log = log;
        }

        public ActionResult Index( string key )
        {
            _log.Debug( String.Format( "Index({0})", key ) );
            return View();
        }

    }
}
