using System;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    public class HtmlFile
    {
        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlAttribute( "file" )]
        public string File { get; set; }
    }
}
