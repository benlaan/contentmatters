using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MvcContrib.Filters;
using Laan.ContentMatters.Models;
using Laan.ContentMatters.Models.Services;
using Laan.ContentMatters.Engine;
using Laan.ContentMatters.Models.Interfaces;

namespace Laan.ContentMatters.Controllers
{
    [Layout( "Site" )]
    public class BaseController : Controller
    {
        protected virtual void InitialiseViewData( object model, string view )
        {
            // for custom views
            ViewData[ "do" ] = new TemplateLoader();

            ViewData[ "now" ] = DateTime.Now;
            ViewData[ "item" ] = model;
            ViewData[ "title" ] = view ?? GetType().Name.Replace( "Controller", "" );
        }

        protected override ViewResult View( IView view, object model )
        {
            InitialiseViewData( model, view.ToString() );
            return base.View( view, model );
        }

        protected override ViewResult View( string viewName, string masterName, object model )
        {
            InitialiseViewData( model, viewName );
            return base.View( viewName, masterName, model );
        }
    }
}
