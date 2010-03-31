using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq;

namespace Laan.ContentMatters.Configuration
{
    [Serializable]
    [XmlRoot( "site" )]
    public class Site
    {
        [XmlElement( "page" )]
        public List<SitePage> Pages { get; set; }

        private void AssignParentToChildPage( SitePage parent, List<SitePage> pages )
        {
            if ( pages == null )
                return;

            foreach ( SitePage childPage in pages )
            {
                childPage.Parent = parent;
                AssignParentToChildPage( childPage, childPage.Pages );
            }
        }

        internal void AssignParentsToChildPages()
        {
            AssignParentToChildPage( null, Pages );
        }

        public SitePage FindSitePages( SitePage page, string path )
        {
            string fileName = String.Format( "{0}.xml", path );

            if ( page.FileName == fileName )
                return page;

            foreach ( SitePage childPage in page.Pages )
            {
                if ( childPage.FileName == fileName )
                    return childPage;
            }

            return null;
        }

        public SitePage FindSitePageByPath( string path )
        {
            foreach ( SitePage page in Pages )
            {
                SitePage result = FindSitePages( page, path );
                if ( result != null )
                    return result;
            }

            return Pages.FirstOrDefault( pg => pg.Default );
        }
    }
}
