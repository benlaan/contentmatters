using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Laan.Utilities.Xml;
using Laan.Library.IO;
using Laan.ContentMatters.Interfaces;

namespace Laan.ContentMatters.Loaders
{
    public interface IViewLoader
    {
        void ProcessNode( XmlNodeList childNodes, StringBuilder html, int level );
    }

    public class ViewLoader : IViewLoader
    {
        private string _appData;
        private Dictionary<string, IHtmlProvider> _providers;

        public ViewLoader( IHtmlProvider[] providers )
        {
            _providers = new Dictionary<string, IHtmlProvider>();
            _appData = @"E:\Development\GoogleCode\Laan.ContentMatters\Laan.ContentMatters.Tests\App_Data";

            if ( providers == null )
                return;

            foreach ( IHtmlProvider provider in providers )
                _providers[provider.ElementName] = provider;
        }

        public void ProcessNode( XmlNodeList childNodes, StringBuilder html, int level )
        {
            foreach ( XmlNode node in childNodes )
            {
                switch ( node.NodeType )
                {
                    case XmlNodeType.Element:
                        ProcessElement( html, level, node );
                        break;

                    case XmlNodeType.CDATA:
                        html.Append( node.Value );
                        break;

                    case XmlNodeType.Comment:
                        html.Append( node.OuterXml );
                        break;
                }
            }
        }

        public void ProcessElement( StringBuilder html, int level, XmlNode node )
        {
            if ( _providers.ContainsKey( node.Name ) )
                html.Append( _providers[node.Name].Render( node ) );
            else
            {
                string indent = new string( ' ', level * 4 );
                html.Append( indent );
                RenderElement( node, html );

                if ( node.HasChildNodes )
                {
                    if ( node.FirstChild.NodeType == XmlNodeType.Text )
                        html.AppendFormat( "{0}</{1}>\n", node.FirstChild.Value, node.Name );
                    else
                    {
                        html.AppendLine();
                        ProcessNode( node.ChildNodes, html, level + 1 );
                        html.AppendFormat( "{0}</{1}>\n", indent, node.Name );
                    }
                }
            }
        }

        private void RenderElement( XmlNode element, StringBuilder html )
        {
            html.AppendFormat( "<{0}", element.Name );
            foreach ( XmlAttribute attr in element.Attributes )
            {
                html.AppendFormat( " {0}=\"{1}\"", attr.Name, attr.Value );
            }
            html.Append( element.HasChildNodes ? ">" : "/>\n" );
        }

        public View Load( string path )
        {
            View result = new View();
            string fullPath = Path.Combine( _appData, "Views", path + ".xml" );
            XmlDocument doc = new XmlDocument();
            doc.Load( fullPath );
            XmlNodeList view = doc.GetElementsByTagName( "view" );
            if ( view == null )
                throw new Exception( String.Format( "can't find '{0}' within views folder", path ) );

            StringBuilder html = new StringBuilder();

            ProcessNode( doc.ChildNodes, html, 0 );
            result.Body = html.ToString();
            return result;
        }
    }
}
