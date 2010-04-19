using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Engine
{
    public abstract class XmlProvider : IXmlProvider
    {
        #region IXmlProvider Members

        public XmlReader ReplaceElement( XmlReader element, IDataDictionary data )
        {
            Data = data;
            MemoryStream ms = new MemoryStream();
            WriteXml( element, ms );
            ms.Position = 0;
            return new XmlTextReader( ms );
        }

        protected void EnsureChildElementExists( XmlReader element, string childElementName )
        {
            element.Read();
            while ( element.NodeType == XmlNodeType.Whitespace )
                element.Read();

            if ( element.Name != childElementName )
                throw new ArgumentException( String.Format( "The {0} provider expects a child element with name {1}", ElementName, childElementName ) );
        }
        
        protected string GetRequiredAttribute( XmlReader input, string attributeName )
        {
            string dataName = input.GetAttribute( attributeName );
            if ( String.IsNullOrEmpty( dataName ) )
                throw new ArgumentNullException( attributeName );
            return dataName;
        }

        protected abstract void WriteXml( XmlReader element, Stream stream );

        public IDataDictionary Data { get; private set; }
        public abstract string ElementName { get; }

        #endregion
    }
}
