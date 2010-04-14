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

        protected override void WriteXml( XmlReader element, Stream stream, IDataDictionary data )
        {
            string dataName = GetRequiredAttribute( element, "data" );
            string classDeclaration = element.GetAttribute( "class" );
            string itemName = element.GetAttribute( "each" ) ?? "$item";

            string pattern = itemName;
            if ( !element.IsEmptyElement)
            {
                element.Read();
                while ( element.NodeType == XmlNodeType.Whitespace )
                {
                    element.Read();
                }

                if ( element.Name != "detail" )
                    throw new ArgumentException( "A list provider must be a simple element or must have a child element" );

                pattern = element.ReadInnerXml();
                element.Read();
            }

            XmlTextWriter writer = new XmlTextWriter( stream, Encoding.UTF8 );
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement( "ul" );
            if ( !String.IsNullOrEmpty( classDeclaration ) )
                writer.WriteAttributeString( "class", classDeclaration );

            object value;             
            if ( data.TryGetValue( dataName, out value ) )
            {
                var items = value as IEnumerable;
                if ( items == null )
                    throw new ArgumentException( "the list data attribute must refernece an IEnuerable object" );

                foreach ( var item in items )
                {
                    writer.WriteStartElement( "li" );

                    string itemKey = itemName.TrimStart( '$' );
                    data.Add( itemKey, (Object)item );
                    writer.WriteRaw( data.UnwrapVariables( pattern ) );
                    data.Remove( itemKey );
                    
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
