using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Principal;
using System.Reflection;

namespace Laan.ContentMatters.Models.Files
{
    public class HttpContextSession : ISession
    {
        private System.Web.HttpSessionStateBase _session;

        public HttpContextSession( System.Web.HttpSessionStateBase session )
        {
            _session = session;
        }

        public object this[ int index ]
        {
            get { return _session[ index ]; }
            set { _session[ index ] = value; }
        }

        public object this[ string name ]
        {
            get { return _session[ name ]; }
            set { _session[ name ] = value; }
        }
    }
}
