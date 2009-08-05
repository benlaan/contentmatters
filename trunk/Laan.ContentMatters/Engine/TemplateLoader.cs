using System;
using System.IO;
using System.Linq;
using Laan.ContentMatters.Models;
using NVelocity;
using NVelocity.App;

namespace Laan.ContentMatters.Engine
{
    public class TemplateLoader 
    {
        /// <summary>
        /// Initializes a new instance of the TemplateLoader class.
        /// </summary>
        /// <param name="masterFolder"></param>
        public TemplateLoader()
        {
        }

        private string GetTargetView( string rootPath, string folder, string viewName )
        {
            var targetView = String.Format( @"..\{0}\{1}\{2}", rootPath, folder, viewName );
            return Path.HasExtension( targetView ) ? targetView : targetView + ".vm";
        }

        protected string ResolveViewTemplate( string controllerFolder, string viewName )
        {
            var folders = new[] { controllerFolder, "_default" };

            string template = null;
            foreach ( string folder in folders )
            {
                template = GetTargetView( "Views", folder, viewName );
                if ( File.Exists( template ) )
                    break;

                template = GetTargetView( "Templates", folder, viewName );
                if ( File.Exists( template ) )
                    break;
            }

            if ( template == null )
            {
                string folderList = String.Join(
                    "\n\t",
                    folders.Select( f => GetTargetView( "Templates", f, viewName ) ).ToArray()
                );

                throw new InvalidOperationException(
                    String.Format( "Could not find view '{0}' at locations:\n\n '{1}'", viewName, folderList )
                );
            }

            return template;
        }

        public string Output( IItem item )
        {
            string action = "Item";
            return Output( item, action );
        }

        public string Output( IItem item, string view )
        {
            string templateName = ResolveViewTemplate( item.TypeName, view );
            VelocityEngine engine = new VelocityEngine();
            engine.Init();

            var viewTemplate = engine.GetTemplate( templateName );
            var context = new VelocityContext();
            context.Put( "item", item );
            context.Put( item.TypeName, item );
            
            using ( StringWriter writer = new StringWriter() )
            {
                viewTemplate.Merge( context, writer );

                return writer.ToString();
            }
        }
    }
}
