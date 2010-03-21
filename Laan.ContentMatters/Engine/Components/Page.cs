using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    [XmlRoot( "page" )]
    public class Page
    {
        public Page()
        {
            Data = new List<Data>();
            Pages = new List<Page>();
        }

        public string Action { get; set; }
        public string Key    { get; set; }
        public Page   Parent { get; set; }
        public Layout Layout { get; set; }

        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlAttribute( "link" )]
        public string Link { get; set; }

        [XmlAttribute( "folder" )]
        public string Folder { get; set; }

        [XmlAttribute( "default" )]
        public bool Default { get; set; }

        [XmlAttribute( "detailPage" )]
        public string DetailPage { get; set; }

        [XmlElement( "page" )]
        public List<Page> Pages { get; set; }

        [XmlElement( "data" )]
        public List<Data> Data { get; set; }
    }
}