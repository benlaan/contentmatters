using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Laan.ContentMatters.Controllers;
using Laan.ContentMatters.Models;
using Laan.Persistence;
using Castle.Core.Logging;
using Laan.Persistence.Interfaces;
using Castle.Facilities.NHibernateIntegration;

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

            Type argumentType = Type.GetType( "Laan.ContentMatters.Models." + controllerName );
            Type type = typeof( Laan.ContentMatters.Controllers.IController<> ).MakeGenericType( argumentType );

            return IoC.Container.Resolve( type ) as IController;
        }

        #endregion
    }
}
