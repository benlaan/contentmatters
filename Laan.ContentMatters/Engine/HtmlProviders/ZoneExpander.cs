using System;
using System.Text;
using System.Xml;

using Laan.Library.IO;
using Laan.ContentMatters.Interfaces;
using Laan.ContentMatters.Loaders;

namespace Laan.ContentMatters.Provider
{
    public class ZoneExpander : IXmlProvider
    {
        private string _appData;

        public ZoneExpander()
        {
            _appData = @"E:\Development\GoogleCode\Laan.ContentMatters\Laan.ContentMatters.Tests\App_Data";
        }

        #region IHtmlProvider Members

        public string ElementName { get { return "zone"; } }

        public XmlReader GetReaderForElement( XmlReader reader )
        {
            var zoneName = reader.GetAttribute( "id" );
            string fullPath = Path.Combine( _appData, "Views", zoneName + ".xml" );
            
            XmlReader result = XmlNodeReader.Create( fullPath );
            result.Read();
            if (result.Name != "view")
                throw new Exception( String.Format( "can't find '{0}' within views folder", zoneName ) );
            
            //XmlDocument doc = new XmlDocument();
            //doc.Load( fullPath );
            //XmlNodeList view = doc.GetElementsByTagName( "view" );
            //if ( view == null )
            //    throw new Exception( String.Format( "can't find '{0}' within views folder", zoneName ) );

            return result;
        }

        #endregion
    }
}
