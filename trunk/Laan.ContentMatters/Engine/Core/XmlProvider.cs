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

        public XmlReader ReplaceElement( XmlReader element, Dictionary<string, object> data )
        {
            element.Read();

            MemoryStream ms = new MemoryStream();
            WriteXml( element, ms, data );
            ms.Position = 0;
            return new XmlTextReader( ms );
        }

        protected string GetRequiredAttribute( XmlReader input, string attributeName )
        {
            string dataName = input.GetAttribute( attributeName );
            if ( String.IsNullOrEmpty( dataName ) )
                throw new ArgumentNullException( attributeName );
            return dataName;
        }

        protected abstract void WriteXml( XmlReader element, Stream stream, Dictionary<string, object> data );
        
        public abstract string ElementName { get; }

        #endregion
    }
}
