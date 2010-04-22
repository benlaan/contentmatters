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

        protected override void WriteXml( XmlReader reader, XmlWriter writer )
        {
            string dataName = GetRequiredAttribute( reader, "data" );
            string itemName = reader.GetAttribute( "each" ) ?? "$item";

            string pattern = itemName;
            if ( !reader.IsEmptyElement )
            {
                EnsureChildElementExists( reader, "detail" );
                pattern = reader.ReadInnerXml();
                reader.Read();
            }

            WriteElementWithOptionalClass( "ul", reader, writer );

            object value;
            if ( Data.TryGetValue( dataName, out value ) )
            {
                if ( value is string )
                    writer.WriteString( ( string )value );

                var items = value as IEnumerable;
                if ( items == null )
                    throw new ArgumentException( "the list data attribute must reference an IEnumerable object" );

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
        }

        #region IXmlProvider Members

        public override string ElementName
        {
            get { return "list"; }
        }

        #endregion
    }
}
