using System;
using System.IO;
using System.Text;
using System.Xml;

using Laan.ContentMatters.Engine;
using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Engine.HtmlProviders
{
    public class WikipediaProvider : XmlProvider
    {
        protected override void WriteXml( XmlReader element, Stream stream, IDataDictionary data )
        {
            var href = element.GetAttribute( "href" );
            element.Read();
            if (!element.IsEmptyElement)
            {
                if (element.NodeType != XmlNodeType.Text)
                    throw new ArgumentException( "a Wiki node must have one text node within it" );
            }

            string text = element.Value;
            href = href ?? text.Replace( ' ', '_' );

            XmlTextWriter writer = new XmlTextWriter( stream, Encoding.UTF8 );
            writer.WriteStartElement( "a" );
            writer.WriteAttributeString( "href", "http://www.wikipedia.org/wiki/" + href );
            writer.WriteAttributeString( "title", "Visit Wikipedia for information about " + text );
            writer.WriteValue( text );
            writer.WriteFullEndElement();
            writer.Flush();
        }

        public override string ElementName
        {
            get { return "wiki"; }
        }
    }
}
