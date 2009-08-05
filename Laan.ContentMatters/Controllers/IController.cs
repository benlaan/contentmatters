using System;
using System.Web.Mvc;

using Laan.ContentMatters.Models;

namespace Laan.ContentMatters.Controllers
{
    public interface IController<T> where T : class, IItem, new()
    {
        //ActionResult Index();
        //ActionResult View( int id );
        //ActionResult Create();
        //ActionResult Create( FormCollection collection );
        //ActionResult Edit( int id );
        //ActionResult Edit( int id, FormCollection collection );
    }
}
