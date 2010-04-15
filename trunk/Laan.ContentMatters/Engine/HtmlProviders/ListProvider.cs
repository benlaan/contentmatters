using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;

using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Engine;

namespace Laan.ContentMatters.Engine.HtmlProviders
{
    public class ListProvider : XmlProvider
    {
        public ListProvider()
        {

        }

        protected override void WriteXml( XmlReader element, Stream stream )
        {
            string dataName = GetRequiredAttribute( element, "data" );
            string classDeclaration = element.GetAttribute( "class" );
            string itemName = element.GetAttribute( "each" ) ?? "$item";

            string pattern = itemName;
            if ( !element.IsEmptyElement )
            {
                EnsureChildElementExists( element, "detail" );
                pattern = element.ReadInnerXml();
                element.Read();
            }

            XmlTextWriter writer = new XmlTextWriter( stream, Encoding.UTF8 );
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement( "ul" );
            if ( !String.IsNullOrEmpty( classDeclaration ) )
                writer.WriteAttributeString( "class", classDeclaration );

            object value;
            if ( Data.TryGetValue( dataName, out value ) )
            {
                if (value is string )
                    writer.WriteString( ( string )value );

                var items = value as IEnumerable;
                if ( items == null )
                    throw new ArgumentException( "the list data attribute must refernece an IEnumerable object" );

                foreach ( var item in items )
                {
                    writer.WriteStartElement( "li" );

                    string itemKey = itemName.TrimStart( '$' );
                    Data.Add( itemKey, item );
                    writer.WriteRaw( Data.ExpandVariables( pattern ) );
                    Data.Remove( itemKey );

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
