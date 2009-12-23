using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Commons.Collections;

using MvcContrib.ViewEngines;

using NVelocity;
using NVelocity.App;

namespace Laan.ContentMatters.Engine
{
    public class TemplatingViewEngine : TemplateLoader, IViewEngine
    {
        private string _masterFolder;
        private VelocityEngine _engine;

        public TemplatingViewEngine()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.RelativeSearchPath;
            Initialise( null );
        }

        public TemplatingViewEngine( IDictionary properties )
        {
            if ( properties == null )
                properties = new Dictionary<string, string>();

            var props = new ExtendedProperties();
            foreach ( string key in properties.Keys )
                props.AddProperty( key, properties[ key ] );

            Initialise( props );
        }

        private void Initialise( ExtendedProperties props )
        {
            _masterFolder = @"\..\Views\Master";
            _engine = new VelocityEngine();
            if ( props != null )
                _engine.Init( props );
            else
                _engine.Init();
        }

        private string GetTargetView( string viewName, string folder )
        {
            var targetView = String.Format( @"..\Templates\{0}\{1}", folder, viewName );
            return Path.HasExtension( targetView ) ? targetView : targetView + ".vm";
        }

        private Template ResolveMasterTemplate( string masterName )
        {
            Template masterTemplate = null;
            if ( string.IsNullOrEmpty( masterName ) )
                return masterTemplate;

            var targetMaster = Path.Combine( _masterFolder, masterName );

            if ( !Path.HasExtension( targetMaster ) )
                targetMaster = targetMaster + ".vm";

            if ( !_engine.TemplateExists( targetMaster ) )
                throw new InvalidOperationException( 
                    String.Format(
                        "Could not find view for master template named {0}. Full path searched was '{1}'",
                        masterName,
                        targetMaster
                    ) 
                );

            return _engine.GetTemplate( targetMaster );
        }

        #region IViewEngine Members

        public ViewEngineResult FindPartialView( ControllerContext controllerContext, string partialViewName, bool useCache )
        {
            return FindView( controllerContext, partialViewName, null, useCache );
        }

        public ViewEngineResult FindView( ControllerContext controllerContext, string viewName, string masterName, bool useCache )
        {
            string controllerName = (string) controllerContext.RouteData.Values[ "controller" ];

            Template viewTemplate = _engine.GetTemplate( ResolveViewTemplate( controllerName, viewName ) );
            Template masterTemplate = ResolveMasterTemplate( masterName );

            NVelocityView view = new NVelocityView( viewTemplate, masterTemplate );
            return new ViewEngineResult( view, this );
        }

        public void ReleaseView( ControllerContext controllerContext, IView view )
        {
            if ( view is IDisposable )
                ( (IDisposable) view ).Dispose();
        }

        #endregion
    }
}
