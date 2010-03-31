using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Laan.ContentMatters.Interfaces;
using System.Xml;

namespace Laan.ContentMatters.Configuration
{
    public class PageLayout
    {
        public PageLayout()
        {
            Views = new List<PageView>();
        }

        [XmlAttribute( "page" )]
        public string Page { get; set; }

        [XmlElement( "view" )]
        public List<PageView> Views { get; set; }

    }
}
