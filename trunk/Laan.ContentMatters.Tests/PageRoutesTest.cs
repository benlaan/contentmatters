using System;
using MbUnit.Framework;
using System.Web.Routing;
using System.Linq.Expressions;
using MvcContrib.TestHelper;
using Laan.ContentMatters.Models.Custom;
using Laan.ContentMatters.Models;
using Laan.ContentMatters.Controllers;
using System.Web.Mvc;
using Laan.ContentMatters.Engine;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class PageRoutesTest : BaseTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            ControllerBuilder.Current.SetControllerFactory( new CustomControllerFactory( null ) );
            //MvcApplication.RegisterRoutes( RouteTable.Routes );
        }

        [Test]
        public void Test()
        {
            // Verify outcome
            //RouteData route = "~/Blog/10/Name".Route();
            //route.ShouldMapTo<ItemController<Blog>>( p => p.View( 10 ) );
        }
    }
}
