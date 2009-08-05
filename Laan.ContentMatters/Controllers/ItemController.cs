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
using Laan.Persistence.Interfaces;
using Castle.Core.Logging;

namespace Laan.ContentMatters.Controllers
{
    public class ItemController<T> : BaseController, IController<T> where T : class, IItem, new()
    {
        protected ILogger _log;
        protected IRepository<T> _repository;

        public ItemController()
        {
        }

        public ItemController( IRepository<T> repository, ILogger log )
        {
            _log = log;
            _repository = repository;
        }

        protected override void InitialiseViewData( object model, string view )
        {
            base.InitialiseViewData( model, view );

            if ( model is IItem )
            {
                IItem item = ( (IItem) model );

                // for master page
                ViewData[ "title" ] = item.Title;

                // for default views
                ViewData[ "item" ] = item;

                // for custom views
                ViewData[ item.TypeName ] = model;
            }
        }

        private ActionResult Show( string view, IItem item )
        {
            if ( item == null )
                return View( view, "Site" );

            return View( view, "Site", item );
        }

        private IItemList<T> GetAll()
        {
            return new ItemList<T>( _repository.GetAll() );
        }

        private T GetOne( int id )
        {
            return _repository.Get( id );
        }

        public ActionResult Index()
        {
            return Show( "Index", GetAll() );
        }

        public ActionResult View( int id )
        {
            return Show( "View", GetOne( id ) );
        }

        public ActionResult Create()
        {
            return Show( "Create", new T() );
        }

        [AcceptVerbs( HttpVerbs.Post )]
        public ActionResult Create( FormCollection collection )
        {
            try
            {
                // TODO: Add insert logic here
                return RedirectToAction( "Index" );
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit( int id )
        {
            T item = GetOne( id );
            return Show( "Create", item );
        }

        [AcceptVerbs( HttpVerbs.Post )]
        public ActionResult Edit( int id, FormCollection collection )
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction( "Index" );
            }
            catch
            {
                return View();
            }
        }
    }
}
