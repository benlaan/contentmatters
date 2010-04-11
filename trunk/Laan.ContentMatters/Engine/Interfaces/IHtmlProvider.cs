using System;
using System.Xml;
using System.Collections.Generic;

namespace Laan.ContentMatters.Engine.Interfaces
{
    public interface IXmlProvider
    {
        XmlReader ReplaceElement( XmlReader element, Dictionary<string, object> data );
        string ElementName { get; }
    }
}
