using System;
using Laan.ContentMatters.Engine;
using MbUnit.Framework;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class PageRoutesTest : BaseTestFixture
    {
        public override void Setup()
        {
            //base.Setup();
            //ControllerBuilder.Current.SetControllerFactory( new CustomControllerFactory( null ) );
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
