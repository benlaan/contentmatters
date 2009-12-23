using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    public class DataSource
    {
        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlAttribute( "type" )]
        public string Type { get; set; }

        [XmlAttribute( "filter" )]
        public string Filter { get; set; }

        [XmlAttribute( "top" )]
        public int Top { get; set; }

        [XmlAttribute( "order" )]
        public string Order { get; set; }
    }

    [Serializable]
    public class Template : HtmlFile
    {
        [XmlArray( "dataSources" ), XmlArrayItem( "dataSource" )]
        public List<DataSource> DataSources { get; set; }
    }
}
