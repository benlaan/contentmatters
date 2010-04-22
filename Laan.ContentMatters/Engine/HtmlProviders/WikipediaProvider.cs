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
        protected override void WriteXml( XmlReader element, XmlWriter writer )
        {
            var href = element.GetAttribute( "href" );
            element.Read();
            if (!element.IsEmptyElement)
            {
                if (element.NodeType != XmlNodeType.Text)
                    throw new ArgumentException( "a Wiki node must have one text node within it" );
            }

            string text = Data.ExpandVariables( element.Value );
            href = href ?? text.Replace( ' ', '_' );

            writer.WriteStartElement( "a" );
            writer.WriteAttributeString( "href", "http://www.wikipedia.org/wiki/" + href );
            writer.WriteAttributeString( "title", "Visit Wikipedia for information about " + text );
            writer.WriteValue( text );
            writer.WriteFullEndElement();
        }

        public override string ElementName
        {
            get { return "wiki"; }
        }
    }
}
