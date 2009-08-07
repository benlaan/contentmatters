using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Laan.ContentMatters.Engine
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        public CustomControllerFactory()
        {

        }

        #region IControllerFactory Members

        public override IController CreateController( RequestContext requestContext, string controllerName )
        {
            string controllerFullName = String.Format( "Laan.ContentMatters.Controllers.{0}Controller", controllerName );
            if ( Type.GetType( controllerFullName ) != null )
                return base.CreateController( requestContext, controllerName );

            //TODO: This needs to be configured as a list of probe paths, and each checked in turn..
            string namespacePrefix = "Laan.ContentMatters.Models.Custom";

            Type argumentType = Type.GetType( String.Format( "{0}.{1}", namespacePrefix, controllerName ), true );
            Type type = typeof( Laan.ContentMatters.Controllers.IController<> ).MakeGenericType( argumentType );

            return IoC.Container.Resolve( type ) as IController;
        }

        #endregion
    }
}
