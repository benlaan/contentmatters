using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.Persistence.Interfaces;
using System.Text.RegularExpressions;

namespace Laan.ContentMatters.Loaders
{
    public class ViewLoader : IViewLoader
    {
        private string _appData;
        private int _indentationSize;
        private IXmlProvider[] _xmlProviders;
        private Dictionary<string, IXmlProvider> _providers;
        private IDataProvider _dataProvider;

        public ViewLoader( IMapper mapper, IDataProvider dataProvider, IDataDictionary data, int indentationSize )
        {
            Data = data;
            _dataProvider = dataProvider;
            _indentationSize = indentationSize;
            _providers = new Dictionary<string, IXmlProvider>();
            _appData = mapper.MapPath( "~/App_Data" );
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
                            writer.WriteFullEndElement();
                        break;

                    case XmlNodeType.Text:
                        writer.WriteRaw( Data.UnwrapVariables( reader.Value ) );
                        break;

                    case XmlNodeType.Comment:
                        writer.WriteComment( reader.Value );
                        break;
                }
            }
        }


        private void WriteElement( XmlReader reader, XmlWriter writer, PageLayout layout )
        {
            switch ( reader.Name )
            {
                case "zone":
                    WriteZone( reader, writer, ref layout );
                    break;

                case "view":
                    reader.Read(); // consume the root element 'view'
                    break;

                default:
                    WriteNodes( reader, writer, layout );
                    break;
            }
        }

        private void WriteZone( XmlReader reader, XmlWriter writer, ref PageLayout layout )
        {
            var zoneName = reader.GetAttribute( "id" );
            if ( String.IsNullOrEmpty( zoneName ) )
                throw new ArgumentNullException( String.Format( "Zone is missing 'id' in layout '{1}'", zoneName, layout.Page ) );

            var pageView = layout.Views.FirstOrDefault( pg => pg.Zone == zoneName );

            if ( pageView == null )
                throw new ArgumentNullException( String.Format( "Zone '{0}' not found in layout '{1}'", zoneName, layout.Page ) );

            if ( pageView.Page == null && pageView.Layout == null )
                throw new ArgumentNullException( String.Format( "Zone {0} has neither page nor layout specified", zoneName ) );

            layout = pageView.Layout;

            string fileName;
            if ( pageView.Layout != null )
                fileName = Laan.Library.IO.Path.Combine( _appData, "Layouts", layout.Page + ".xml" );
            else
                fileName = Laan.Library.IO.Path.Combine( _appData, "Views", pageView.Page + ".xml" );

            XmlReaderSettings settings = new XmlReaderSettings();

            using ( XmlReader zoneReader = XmlNodeReader.Create( fileName, settings ) )
            {
                ProcessNodes( zoneReader, writer, layout );
            }
        }

        private void WriteNodes( XmlReader reader, XmlWriter writer, PageLayout layout )
        {
            IXmlProvider provider;
            string name = reader.Name;
            if ( _providers.TryGetValue( name, out provider ) )
            {
                using ( var render = provider.ReplaceElement( reader, Data ) )
                {
                    ProcessNodes( render, writer, layout );
                    if ( !reader.IsEmptyElement )
                    {
                        while ( reader.NodeType != XmlNodeType.EndElement )
                            reader.Read();

                        if ( reader.Name != name )
                            throw new Exception( "Xml Reader mismatch" );
                    }
                }
            }
            else
            {
                bool isEmpty = reader.IsEmptyElement;

                writer.WriteStartElement( reader.Name );
                for ( int index = 0; index < reader.AttributeCount; index++ )
                {
                    reader.MoveToAttribute( index );
                    writer.WriteStartAttribute( reader.Name );
                    writer.WriteString( Data.UnwrapVariables( reader.Value ) );
                    writer.WriteEndAttribute();
                }

                if ( isEmpty )
                {
                    if ( reader.Name == "div" )
                        writer.WriteFullEndElement(); // workaround for HTML limitation due to supporting <div/>
                    else
                        writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Providers is supplied via property injection using Castle
        /// </summary>
        public IXmlProvider[] Providers
        {
            get { return _xmlProviders; }
            set
            {
                _xmlProviders = value;
                if ( _xmlProviders == null )
                    return;

                _providers.Clear();
                foreach ( IXmlProvider provider in _xmlProviders )
                    _providers[provider.ElementName] = provider;
            }
        }

        #region IViewLoader Members

        public View Load( Page page )
        {
            string fullPath = Laan.Library.IO.Path.Combine( _appData, "Layouts", page.Layout.Page + ".xml" );

            View view = new View();
            view.Html = GenerateHtml( fullPath, page.Layout );
            view.Data = Data;
            return view;
        }

        public void GenerateData( Page page, IDictionary<string, object> contextData )
        {
            Data = _dataProvider.Build( page );
            if (contextData == null)
                return;

            foreach ( string key in contextData.Keys )
                Data[key] = contextData[key];
        }

        public IDataDictionary Data { get; set; }

        #endregion
    }
}
