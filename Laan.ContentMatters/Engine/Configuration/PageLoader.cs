using System;
using System.Collections.Generic;
using System.Linq;

using Laan.ContentMatters.Engine;

namespace Laan.ContentMatters.Configuration
{
    public class PageLoader
    {
        private PageConfiguration _configuration;

        public PageLoader( PageConfiguration configuration )
        {
            _configuration = configuration;
        }

        private Page FindDefaultPage( string path )
        {
            Page defaultPage = _configuration.Pages.FirstOrDefault( pg => pg.Default );
            if ( defaultPage != null )
                return defaultPage;
            else
                throw new PageNotFoundException( path );
        }

        private Template FindTemplate( Page page )
        {
            string templateName = page.GetTemplateName();
            var template = _configuration.Templates.FirstOrDefault( tmp => tmp.Name == templateName );
            if ( template == null )
                throw new TemplateNotFoundException( templateName );

            return template;
        }

        private Layout FindLayout( Page page )
        {
            string layoutName = page.GetLayoutName();
            var layout = _configuration.Layouts.FirstOrDefault( tmp => tmp.Name == layoutName );
            if ( layout == null )
                throw new LayoutNotFoundException( layoutName );

            return layout;
        }

        public Page GetPageFromPath( string path )
        {
            string[] actionList = new[] { "index", "list", "edit", "delete", "new" };
            string[] folders = path.ToLower().Trim( '/' ).Split( new[] { "/" }, StringSplitOptions.RemoveEmptyEntries );
            
            Page page = null;
            Page parent = null;
            IList<Page> childPages = _configuration.Pages;

            foreach ( string folder in folders )
            {
                Page childPage = childPages.FirstOrDefault( pg => pg.Name == folder );
                page = childPage ?? page ?? FindDefaultPage( path );

                // assume that if the page can't be found, the 'folder' is actually either
                // an action or a key. If it can be found then assign it a parent page
                if ( childPage == null )
                {
                    if ( actionList.Any( action => action == folder ) )
                        page.Action = folder;
                    else
                        page.Key = folder.Replace( '-', ' ' );
                }
                else
                    page.Parent = parent;

                childPages = page.Pages;
                parent = page;
            }

            // if nothing has been found yet, use the default page
            page = page ?? FindDefaultPage( path );

            // match template and layout by name
            page.Template = FindTemplate( page );
            page.Layout = FindLayout( page );

            return page;
        }
    }
}
