using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    [DebuggerDisplay( "{Name}: {FullPath}" )]
    public class SitePage
    {
        private string _link;

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
                if (_link != null)
                    return _link;

                string parentLink = Parent != null ? Parent.Link : "";
                return Laan.Library.IO.Path.Combine( parentLink, Name );
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
        public Page Page { get; private set; }
        
        [XmlIgnore]
        public SitePage Parent { get; set; }
        
        [XmlIgnore]
        public string Title { get; set; }
        
        [XmlIgnore]
        public string Key { get; set; }
        
        [XmlIgnore]
        public string Action { get; set; }
        
        [XmlIgnore]
        public string Description { get; set; }

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

        public void CopyFromPage(Page page)
        {
            Page = page;
            Description = page.Description;
            Name = page.Name;
            Title = page.Title;
            Action = page.Action;
            Key = page.Key;
            
        }

    }
}
