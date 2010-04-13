using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters
{
    public class View : System.Web.Mvc.IView
    {
        public View()
        {
        }

        public IDataDictionary Data { get; set; }
        public string Html { get; set; }

        #region IView Members

        public void Render( ViewContext viewContext, TextWriter writer )
        {
            writer.Write( Html );
        }

        #endregion
    }
}
