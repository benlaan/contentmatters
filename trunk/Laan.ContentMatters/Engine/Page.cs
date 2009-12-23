using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    public class Property
    {
        [XmlAttribute( "key" )]
        public string Key { get; set; }

        [XmlAttribute( "value" )]
        public string Value { get; set; }
    }

    [Serializable]
    public class Page
    {
        public string Action { get; set; }
        public string Key { get; set; }
        public Page Parent { get; set; }
        public Template Template { get; set; }
        public Layout Layout { get; set; }

        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlAttribute( "template" )]
        public string TemplateName { get; set; }

        [XmlAttribute( "link" )]
        public string Link { get; set; }

        [XmlAttribute( "layout" )]
        public string LayoutName { get; set; }

        [XmlAttribute( "default" )]
        public bool Default { get; set; }

        [XmlElement( "page" )]
        public List<Page> Pages { get; set; }

        internal string GetTemplateName()
        {
            string templateName = "";
            Page page = this;
            do
            {
                templateName = page.TemplateName;
                page = page.Parent;
            }
            while ( page != null && String.IsNullOrEmpty( templateName ) );

            return templateName;
        }

        internal string GetLayoutName()
        {
            string layoutName = "";
            Page page = this;
            do
            {
                layoutName = page.LayoutName;
                page = page.Parent;
            }
            while ( page != null && String.IsNullOrEmpty( layoutName ) );

            return layoutName;
        }
    }
}
