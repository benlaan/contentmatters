using System;
using System.Text;
using System.Xml;

using Laan.Library.IO;
using Laan.ContentMatters.Interfaces;
using Laan.ContentMatters.Loaders;

namespace Laan.ContentMatters.Provider
{
    public class ZoneExpander : IHtmlProvider
    {
        private string _appData;
        private IViewLoader _viewLoader;

        public ZoneExpander( IViewLoader viewLoader )
        {
            _viewLoader = viewLoader;
            _appData = @"E:\Development\GoogleCode\Laan.ContentMatters\Laan.ContentMatters.Tests\App_Data";
        }

        #region IHtmlProvider Members

        public string ElementName { get { return "zone"; } }

        public string Render( XmlNode node )
        {
            string zoneName = node.Attributes["zone"].Value;

            string fullPath = Path.Combine( _appData, "Views", zoneName + ".xml" );
            XmlDocument doc = new XmlDocument();
            doc.Load( fullPath );
            XmlNodeList view = doc.GetElementsByTagName( "view" );
            if ( view == null )
                throw new Exception( String.Format( "can't find '{0}' within views folder", zoneName ) );

            StringBuilder html = new StringBuilder();
            _viewLoader.ProcessNode( doc.ChildNodes, html, 0 );
            return html.ToString();
        }

        #endregion
    }
}
