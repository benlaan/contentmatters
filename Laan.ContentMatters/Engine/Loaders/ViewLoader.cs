using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Laan.Utilities.Xml;
using Laan.Library.IO;
using Laan.ContentMatters.Interfaces;
using System.IO;
using System.Diagnostics;
using Laan.ContentMatters.Configuration;

namespace Laan.ContentMatters.Loaders
{
    public class ViewLoader
    {
        private string _appData;
        private int _indentationSize;
        private Dictionary<string, IXmlProvider> _providers;

        public ViewLoader( IXmlProvider[] providers, int indentationSize )
        {
            _indentationSize = indentationSize;
            _providers = new Dictionary<string, IXmlProvider>();
            _appData = @"E:\Development\GoogleCode\Laan.ContentMatters\Laan.ContentMatters.Tests\App_Data";

            if ( providers == null )
                return;

            foreach ( IXmlProvider provider in providers )
                _providers[provider.ElementName] = provider;
        }

        public View Load( PageLayout layout )
        {
            string fullPath = Laan.Library.IO.Path.Combine( _appData, "Layouts", layout.Page + ".xml" );

            View view = new View();
            view.Html = GenerateHtml( fullPath, layout );
            //view.Data = BuildData();
            return view;
        }

        private string GenerateHtml( string fileName, PageLayout layout )
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            using ( XmlReader reader = XmlNodeReader.Create( fileName, settings ) )
            {
                using ( MemoryStream ms = new MemoryStream() )
                {
                    using ( XmlTextWriter writer = new XmlTextWriter( ms, Encoding.UTF8 ) )
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.Indentation = _indentationSize;
                        writer.WriteStartDocument();
                        ProcessNodes( reader, writer, layout );
                        writer.Flush();

                        using ( StreamReader sr = new StreamReader( ms, Encoding.ASCII ) )
                        {
                            ms.Position = 0;
                            return sr.ReadToEnd().Replace( "\r", "" );
                        }
                    }
                }
            }
        }

        private void ProcessNodes( XmlReader reader, XmlWriter writer, PageLayout layout )
        {
            while ( !reader.EOF )
            {
                reader.Read();
                switch ( reader.NodeType )
                {
                    case XmlNodeType.Element:
                        WriteElement( reader, writer, layout );

                        break;

                    case XmlNodeType.EndElement:
                        if ( reader.Name != "view" )
                            writer.WriteEndElement();
                        break;

                    case XmlNodeType.Text:
                        writer.WriteRaw( reader.Value );
                        break;
                }
            }
        }

        private void WriteElement( XmlReader reader, XmlWriter writer, PageLayout layout )
        {
            switch ( reader.Name )
            {
                case "zone":
                    var zoneName = reader.GetAttribute( "id" );
                    var pageView = layout.Views.FirstOrDefault( pg => pg.Zone == zoneName );

                    if ( pageView == null )
                        throw new Exception( String.Format( "Zone {0} not found", zoneName ) );

                    layout = pageView.Layout;
                    string fileName = Laan.Library.IO.Path.Combine( _appData, "Views", pageView.Page + ".xml" );
                    XmlReaderSettings settings = new XmlReaderSettings();

                    using ( XmlReader zoneReader = XmlNodeReader.Create( fileName, settings ) )
                    {
                        ProcessNodes( zoneReader, writer, layout );
                    }
                    break;

                case "view":
                    reader.Read(); // consume the root element 'view'
                    break;

                default:
                    IXmlProvider provider;
                    if ( _providers.TryGetValue( reader.Name, out provider ) )
                    {
                        var render = provider.GetReaderForElement( reader );
                        ProcessNodes( render, writer, layout );
                    }
                    else
                    {
                        writer.WriteStartElement( reader.Name );
                        writer.WriteAttributes( reader, false );

                        if ( reader.IsEmptyElement )
                            writer.WriteEndElement();
                    }
                    break;
            }
        }
    }
}
