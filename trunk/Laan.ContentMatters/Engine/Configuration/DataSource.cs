using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    public class Data
    {
        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlAttribute( "type" )]
        public string Type { get; set; }

        [XmlAttribute( "select" )]
        public SelectionMode Select { get; set; }

        [XmlAttribute( "top" )]
        public int Top { get; set; }

        [XmlAttribute( "order" )]
        public string Order { get; set; }
    }
}
