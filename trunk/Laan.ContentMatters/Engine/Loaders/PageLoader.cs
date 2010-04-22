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
            string fileName = Path.Combine( _appData, "Pages", page.FileName + ".xml" );
            if (System.IO.File.Exists( fileName ))
                return XmlPersistence<Page>.LoadFromFile( fileName );
            else
                return null;;
        }

        //internal Page LoadPageFromFile( string path )
        //{
        //    LoadSite();
        //    SitePage sitePage = Site.FindSitePageByPath( path );
        //    if ( sitePage == null )
        //        throw new Exception( String.Format( "Page Not Found", path ) );

        //    return LoadPage( sitePage );
        //}

        private void LoadChildPages( IList<SitePage> childPages )
        {
            foreach ( SitePage childSitePage in childPages )
            {
                Page child = LoadPage( childSitePage ) ?? new Page();
                childSitePage.Page = child;
            }
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

            LoadChildPages( childPages );

            foreach ( string folder in folders )
            {
                parentPage = page;
                SitePage childPage = childPages.FirstOrDefault( pg => String.Compare( pg.Name, folder, true ) == 0 );

                if ( childPage != null )
                {
                    sitePage = childPage;
                    page = sitePage.Page;

                    sitePage.Parent = parentSitePage;
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
                LoadChildPages( childPages );
                parentSitePage = sitePage;
                //parentSitePage.Page = page;
            }

            if ( page == null )
            {
                sitePage = FindDefaultPage( path );
                if ( sitePage != null )
                    page = sitePage.Page;

                if (page == null)
                    throw new PageNotFoundException( path );
                
            }
            sitePage.Page.Action = sitePage.Page.Action ?? ( sitePage.Page.Key != null ? "detail" : "index" );
            return sitePage;
        }

        public Site Site { get; private set; }
    }
}
