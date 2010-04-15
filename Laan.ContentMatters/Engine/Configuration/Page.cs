using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    [XmlRoot( "page" )]
    public class Page
    {
        private string _description;
        private string _title;
        
        public Page()
        {
            DataSources = new List<DataSource>();
            Layout = new PageLayout();
            Action = "index";
        }

        public string Action { get; set; }
        public string Key    { get; set; }
        public SitePage Parent { get; set; }

        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlAttribute( "description" )]
        public string Description
        {
            get { return _description ?? Title; }
            set { _description = value; }
        }

        [XmlAttribute( "title" )]
        public string Title
        {
            get { return _title ?? Name; }
            set { _title = value; }
        }

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