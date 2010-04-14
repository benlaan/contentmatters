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

        public PageLoader( IMapper mapper )
        {
            _appData = mapper.MapPath( "~/App_Data" );
        }

        public PageLoader( IMapper mapper, Site site ) : this(mapper)
        {
            Site = site;
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

        public Page GetPageFromPath( string path )
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
                SitePage childPage = childPages.FirstOrDefault( pg => pg.Name == folder );

                if ( childPage != null )
                {
                    sitePage = childPage;
                    page = LoadPage( sitePage );
                    page.Parent = parentPage;
                    sitePage.Parent = parentSitePage;
                }
                else
                {
                    if ( page == null )
                        throw new PageNotFoundException( "Error: 404 - File Not Found" );

                    // assume that if the page can't be found, the 'folder' is actually either
                    // an action or a key. If it can be found then assign it a parent page
                    if ( actionList.Any( action => action == folder ) )
                        page.Action = folder;
                    else
                        page.Key = folder.Replace( '-', ' ' );
                }

                childPages = sitePage.Pages;
                parentSitePage = sitePage;
            }

            if ( page == null )
            {
                SitePage defaultPage = FindDefaultPage( path );
                if ( defaultPage != null )  
                    page = LoadPage( defaultPage );
                else
                    throw new PageNotFoundException( "No Default Page Found" );
            }

            page.Action = page.Action ?? "index";

            return page;
        }

        public Site Site { get; private set; }
    }
}
