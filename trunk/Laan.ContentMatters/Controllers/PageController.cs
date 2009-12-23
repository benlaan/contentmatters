using System;

using log4net.Core;

using Laan.Persistence.Interfaces;
using Laan.ContentMatters.Models;
using System.Web.Mvc;

namespace Laan.ContentMatters.Controllers
{
    public class PageController<T> : BaseController //, IController<Item>
    {
        protected ILogger _log;
        protected IRepository<T> _repository;

        public PageController()
        {
        }

        public PageController( IRepository<T> repository, ILogger log )
        {
            _log = log;
            _repository = repository;
        }

        internal ActionResult Index( string page )
        {
            return View();
        }
    }
}
