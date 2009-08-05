using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Principal;
using System.Reflection;

namespace Laan.ContentMatters.Models.Files
{
    public class HttpContext : IContext
    {
        private System.Web.HttpContextBase _context;

        public HttpContext( System.Web.HttpContextBase context )
        {
            _context = context;
            Session = new HttpContextSession( context.Session );
        }

        #region IContext Members

        public ISession Session { get; set; }

        public IPrincipal Principal
        {
            get { return _context.User; }
        }

        #endregion
    }
}
