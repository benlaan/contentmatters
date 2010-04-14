using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    [XmlRoot( "page" )]
    public class Page
    {
        public Page()
        {
            DataSources = new List<DataSource>();
            Layout = new PageLayout();
        }

        public string Action { get; set; }
        public string Key    { get; set; }
        public Page   Parent { get; set; }

        [XmlAttribute( "name" )]
        public string Name { get; set; }

        //[XmlAttribute( "description" )]
        //public string Description { get; set; }

        //[XmlAttribute( "folder" )]
        //public string Folder { get; set; }

        //[XmlAttribute( "detailPage" )]
        //public string DetailPage { get; set; }

        [XmlElement( "layout" )]
        public PageLayout Layout { get; set; }

        [XmlElement( "data" )]
        public List<DataSource> DataSources { get; set; }
    }
}