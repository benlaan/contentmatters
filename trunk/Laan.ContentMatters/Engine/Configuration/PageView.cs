using System;
using System.Xml.Serialization;
using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    public class PageView
    {
        [XmlAttribute( "page" )]
        public string Page { get; set; }

        [XmlAttribute( "zone" )]
        public string Zone { get; set; }

        [XmlElement( "layout" )]
        public PageLayout Layout { get; set; }
    }
}
