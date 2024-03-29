using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    [DebuggerDisplay( "{Title}: {FullPath}" )]
    public class SitePage
    {
        private string _link;
        private string _title;

        public SitePage()
        {
            Pages = new List<SitePage>();
        }

        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlAttribute( "link" )]
        public string Link
        {
            get
            {
                if ( _link != null )
                    return _link;

                string parentLink = Parent != null ? Parent.Link : "";
                return String.Format( "{0}/{1}", parentLink, Page != null ? Page.Name : null );
            }
            set { _link = value; }
        }

        [XmlAttribute( "folder" )]
        public string Folder { get; set; }

        [XmlAttribute( "default" )]
        public bool Default { get; set; }

        [XmlElement( "page" )]
        public List<SitePage> Pages { get; set; }

        [XmlIgnore]
        public Page Page { get; internal set; }

        [XmlIgnore]
        public SitePage Parent { get; set; }

        [XmlAttribute( "title" )]
        public string Title
        {
            get { return !String.IsNullOrEmpty( _title ) ? _title : Page != null ? Page.Title : null; }
            set { _title = value; }
        }

        [XmlIgnore]
        public string Key { get { return Page != null ? Page.Key : null; } }

        [XmlIgnore]
        public string Action { get { return Page.Action; } }

        [XmlIgnore]
        public string Description { get { return Page.Description; } }

        public string FullPath
        {
            get
            {
                string parentPath = Parent != null ? Parent.FullPath : "";
                return Laan.Library.IO.Path.Combine( parentPath, Folder ?? "" );
            }
        }

        public string FileName
        {
            get { return Laan.Library.IO.Path.Combine( FullPath, Name ); }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
