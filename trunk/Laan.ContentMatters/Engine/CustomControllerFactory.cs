using System;
using System.Web.Mvc;
using System.Web.Routing;

using Castle.MicroKernel;


namespace Laan.ContentMatters.Engine
{   
    public class CustomControllerFactory : DefaultControllerFactory
    {
        private IKernel _container;

        public CustomControllerFactory( IKernel container )
        {
            _container = container;
        }

        #region IControllerFactory Members

        public override IController CreateController( RequestContext requestContext, string controllerName )
        {
            string controllerFullName = String.Format( "Laan.ContentMatters.Controllers.{0}Controller", controllerName );
            if ( Type.GetType( controllerFullName, false, true ) != null )
                return base.CreateController( requestContext, controllerName );

            //TODO: This needs to be modified to be driven by a configuration setting
            var probePaths = new[]
            { 
                new { Namespace = "Laan.ContentMatters.Models.Custom", Assembly = "Laan.ContentMatters.Models.Custom" },
                new { Namespace = "Laan.ContentMatters.Models", Assembly = "Laan.ContentMatters" }
            };

            Type type = null;
            foreach ( var probePath in probePaths )
            {
                var argumentType = Type.GetType(
                    String.Format( "{0}.{1}, {2}", probePath.Namespace, controllerName, probePath.Assembly ),
                    false,
                    true
                );

                if ( argumentType != null )
                {
                    type = typeof( Laan.ContentMatters.Controllers.IController<> ).MakeGenericType( argumentType );
                    break;
                }
            }
            if ( type == null )
                throw new NullReferenceException( String.Format( "Controller<{0}> not found", controllerName ) );

            var controller = _container.Resolve( type ) as IController;
            if ( controller == null )
                throw new NullReferenceException( String.Format( "Controller<{0}> not found in container", controllerName ) );

            return controller;
        }

        #endregion
    }
}
