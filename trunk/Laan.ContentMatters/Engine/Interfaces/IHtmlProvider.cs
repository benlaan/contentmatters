using System;
using System.Xml;

namespace Laan.ContentMatters.Interfaces
{
    public interface IXmlProvider
    {
        XmlReader GetReaderForElement( XmlReader reader );
        string ElementName { get; }
    }
}
