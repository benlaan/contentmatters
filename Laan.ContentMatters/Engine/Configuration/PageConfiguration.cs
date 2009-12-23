using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Linq;

using Laan.ContentMatters.Engine;

namespace Laan.ContentMatters.Configuration
{
    [Serializable, XmlRoot( "pageConfiguration" )]
    public class PageConfiguration
    {
        public PageConfiguration()
        {
            Pages = new List<Page>();
            Templates = new List<Template>();
            Layouts = new List<Layout>();
        }

        [XmlArray( "pages" ), XmlArrayItem( "page" )]
        public List<Page> Pages { get; set; }

        [XmlArray( "templates" ), XmlArrayItem( "template" )]
        public List<Template> Templates { get; set; }

        [XmlArray( "layouts" ), XmlArrayItem( "layout" )]
        public List<Layout> Layouts { get; set; }
    }
}