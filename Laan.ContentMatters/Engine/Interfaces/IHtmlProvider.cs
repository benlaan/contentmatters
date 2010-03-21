using System;
using System.Xml;

namespace Laan.ContentMatters.Interfaces
{
    public interface IHtmlProvider
    {
        string ElementName { get; }
        string Render( XmlNode node );
    }
}
