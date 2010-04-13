using System;
using System.Collections.Generic;
using System.Linq;
using Laan.ContentMatters.Engine;
using Laan.Utilities.Xml;
using Laan.Library.IO;
using Laan.ContentMatters.Configuration;
using Laan.Persistence.Interfaces;

namespace Laan.ContentMatters.Loaders
{
    public class ErrorPage : Page
    {
        public ErrorPage( int code, string path )
        {
            Layout.Page = "master";
            Layout.Views.Add( new PageView { Zone = "main", Page = "error" } );
        }
    }
}
