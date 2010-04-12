using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;

using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Engine.HtmlProviders
{
    public class ListProvider : XmlProvider, IXmlProvider
    {
        public ListProvider()
        {

        }

        protected override void WriteXml( XmlReader element, Stream stream, Dictionary<string, object> data )
        {
            string dataName = GetRequiredAttribute( element, "data" );
            string classDeclaration = element.GetAttribute( "class" );
            
            XmlTextWriter writer = new XmlTextWriter( stream, Encoding.UTF8 );
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement( "ul" );
            if (!String.IsNullOrEmpty(classDeclaration))
                writer.WriteAttributeString( "class", classDeclaration );

            object item;
            if ( data.TryGetValue(dataName, out item) )
            {
                var listdata = item as IEnumerable;
                if ( listdata != null )
                    foreach ( object datum in ( IEnumerable )data[ dataName ] )
                    {
                        writer.WriteStartElement( "li" );
                        writer.WriteValue( datum );
                        writer.WriteEndElement();
                    }
            }

            writer.WriteFullEndElement();
            writer.Flush();
        }

        #region IXmlProvider Members

        public override string ElementName
        {
            get { return "list"; }
        }

        #endregion
    }
}
