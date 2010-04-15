using System;
using System.Collections.Generic;
using System.Linq;

using Laan.ContentMatters.Engine;
using Laan.Utilities.Xml;
using Laan.Library.IO;
using Laan.ContentMatters.Configuration;
using Laan.Persistence.Interfaces;

namespace Laan.ContentMatters.Loaders
{
    public class PageLoader
    {
        private string _appData;
        private Laan.ContentMatters.Engine.Interfaces.ISiteProperties _siteProperties;

        public PageLoader( IMapper mapper, Laan.ContentMatters.Engine.Interfaces.ISiteProperties siteProperties )
        {
            _siteProperties = siteProperties;
            _appData = mapper.MapPath( "~/App_Data" );
        }

        private SitePage FindDefaultPage( string path )
        {
            SitePage defaultPage = Site.Pages.FirstOrDefault( pg => pg.Default );
            if ( defaultPage != null )
                return defaultPage;
            else
                throw new PageNotFoundException( path );
        }

        private void LoadSite()
        {
            if ( Site != null )
                return;

            string sitePath = Path.Combine( _appData, "Pages", "site.xml" );
            Site = XmlPersistence<Site>.LoadFromFile( sitePath );
            Site.AssignParentsToChildPages();
            Site.LoadProperties( _siteProperties );
        }

        private Page LoadPage( SitePage page )
        {
            return XmlPersistence<Page>.LoadFromFile( Path.Combine( _appData, "Pages", page.FileName + ".xml" ) );
        }

        internal Page LoadPageFromFile( string path )
        {
            LoadSite();
            SitePage sitePage = Site.FindSitePageByPath( path );
            if ( sitePage == null )
                throw new Exception( String.Format( "Page Not Found", path ) );

            return LoadPage( sitePage );
        }

        public SitePage GetPageFromPath( string path )
        {
            LoadSite();

            string[] actionList = new[] { "index", "list", "edit", "delete", "new" };
            string[] folders = path.ToLower().Trim( '/' ).Split( new[] { "/" }, StringSplitOptions.RemoveEmptyEntries );

            Page parentPage = null;
            SitePage sitePage = null;
            SitePage parentSitePage = null;
            IList<SitePage> childPages = Site.Pages;
            Page page = null;

            foreach ( string folder in folders )
            {
                parentPage = page;
                SitePage childPage = childPages.FirstOrDefault( pg => String.Compare( pg.Name, folder, true ) == 0 );

                if ( childPage != null )
                {
                    sitePage = childPage;
                    page = LoadPage( sitePage );

                    sitePage.Parent = parentSitePage;
                    sitePage.CopyFromPage( page );
                }
                else
                {
                    if ( page == null )
                        throw new PageNotFoundException( "Error: 404 - File Not Found" );

                    // assume that if the page can't be found, the 'folder' is actually either
                    // an action or a key. If it can be found then assign it a parent page
                    if ( actionList.Any( action => String.Compare( action, folder, true ) == 0 ) )
                        page.Action = folder;
                    else
                        page.Key = folder.Replace( '-', ' ' );
                }

                childPages = sitePage.Pages;
                parentSitePage = sitePage;
                parentSitePage.CopyFromPage( page );
            }

            if ( page == null )
            {
                sitePage = FindDefaultPage( path );
                if ( sitePage != null )
                {
                    page = LoadPage( sitePage );
                    sitePage.CopyFromPage( page );
                }
                else
                    throw new PageNotFoundException( "No Default Page Found" );
            }

            return sitePage;
        }

        public Site Site { get; private set; }
    }
}
