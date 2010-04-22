using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using Laan.ContentMatters.Engine.Interfaces;
using System.Diagnostics;
using Laan.ContentMatters.Engine.HtmlProviders;

namespace Laan.ContentMatters.Engine
{
    public abstract class XmlProvider : IXmlProvider
    {

        private Formatting _formatting;
        private XmlParserContext _context;

        #region IXmlProvider Members

        /// <summary>
        /// Initializes a new instance of the XmlProvider class.
        /// </summary>
        public XmlProvider()
        {
            _context = new XmlParserContext( null, null, "", XmlSpace.Default );
        }

        [Conditional("DEBUG")]
        private static void WriteStream( MemoryStream ms )
        {
            StreamReader sr = new StreamReader( ms );
            Trace.WriteLine( sr.ReadToEnd() );
            ms.Position = 0;
        }

        protected void SetFormatting( XmlWriter writer, Formatting formatting )
        {
            XmlTextWriter textWriter = writer as XmlTextWriter;
            _formatting = textWriter.Formatting;
            textWriter.Formatting = formatting;
        }

        protected void ResetFormatting( XmlWriter writer )
        {
            XmlTextWriter textWriter = writer as XmlTextWriter;
            textWriter.Formatting = _formatting;
        }

        protected void WriteElementWithOptionalClass( string elementName, XmlReader reader, XmlWriter writer )
        {
            string classDeclaration = reader.GetAttribute( "class" );
            writer.WriteStartElement( elementName );
            if ( !String.IsNullOrEmpty( classDeclaration ) )
                writer.WriteAttributeString( "class", classDeclaration );
        }

        protected void EnsureChildElementExists( XmlReader reader, string childElementName )
        {
            reader.Read();
            while ( reader.NodeType == XmlNodeType.Whitespace )
                reader.Read();

            if ( reader.Name != childElementName )
                throw new ArgumentException( String.Format( "The {0} provider expects a child element with name {1}", ElementName, childElementName ) );
        }

        protected string GetRequiredAttribute( XmlReader input, string attributeName )
        {
            string dataName = input.GetAttribute( attributeName );
            if ( String.IsNullOrEmpty( dataName ) )
                throw new ArgumentNullException( attributeName );
            return dataName;
        }

        protected virtual XmlNodeType GetFragmentType()
        {
            return XmlNodeType.Element;
        }

        protected abstract void WriteXml( XmlReader reader, XmlWriter writer );

        public XmlReader ReplaceElement( XmlReader element, IDataDictionary data )
        {
            Data = data;
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter( ms, Encoding.UTF8 );
            writer.Formatting = Formatting.Indented;
            WriteXml( element, writer );
            writer.Flush();
            ms.Position = 0;

            WriteStream( ms );
            return new XmlTextReader( ms, GetFragmentType(), _context );
        }

        public IDataDictionary Data { get; private set; }
        public abstract string ElementName { get; }

        #endregion
    }
}
