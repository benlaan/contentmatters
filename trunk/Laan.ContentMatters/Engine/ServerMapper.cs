using System;
using System.Web.Hosting;

using Laan.Persistence.Interfaces;

namespace Laan.ContentMatters.Engine
{
    public class ServerMapper : IMapper
    {
        public ServerMapper() { }

        #region IMapper Members

        public string MapPath( string path )
        {
            return HostingEnvironment.MapPath( path );
        }

        #endregion
    }
}
