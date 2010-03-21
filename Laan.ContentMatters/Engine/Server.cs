using System;
using System.Web.Hosting;

using Laan.Persistence.AutoMapping;

namespace Laan.ContentMatters.Engine
{
    public class Server : IServer
    {
        public Server()
        {

        }

        #region IServer Members

        public string MapPath(string path)
        {
            return HostingEnvironment.MapPath(path);
        }

        #endregion
    }
}
