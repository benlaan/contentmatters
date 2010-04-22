using System;
using Laan.ContentMatters.Configuration;
using MbUnit.Framework;
using Laan.Utilities.Xml;
using Laan.ContentMatters.Engine;
using Laan.Persistence.Interfaces;
using Rhino.Mocks;
using System.Collections.Generic;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Loaders;

namespace Laan.ContentMatters.Tests
{

    [ TestFixture ]
    public class PagePathTest : BaseTestFixture
    {
        private PageLoader _pageLoader;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            IMapper mapper = _mock.DynamicMock<IMapper>();
            Expect.Call( mapper.MapPath( "" ) ).IgnoreArguments().Return( @".\App_Data\" ).Repeat.Any();

            _mock.ReplayAll();
            ISiteProperties siteProperties = new SiteProperties(new Dictionary<string, object>());
            _pageLoader = new PageLoader( mapper, siteProperties );
        }

        [Test]
        [Row( "" )]
        [Row( "/" )]
        public void Blank_Page_Loads_Default( string path )
        {
            SitePage page = _pageLoader.GetPageFromPath( path );
            Assert.AreEqual(page.Name, "home");
            Assert.AreEqual( "index", page.Action );
        }

        [Test]
        public void Blog_Page_With_Name_Selected()
        {
            string path = "/Blogs/My-Blog";

            // Exercise
            SitePage page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "blogs", page.Name );
            Assert.AreEqual( "my blog", page.Key );
            Assert.AreEqual( "detail", page.Action );
        }

        [Test]
        public void Blog_Child_Post_Page_With_Verb()
        {
            string path = "/Blogs/My-Blog/Posts/A-Short-Thought/Edit";

            // Exercise
            SitePage page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "posts", page.Name );
            Assert.AreEqual( "edit", page.Action );
        }

        [Test]
        public void Blog_Child_Post_Page_With_Name()
        {
            string path = "/Blogs/My-Blog/Posts/A-Short-Thought";

            // Exercise
            SitePage page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "posts", page.Name );
            Assert.AreEqual( "detail", page.Action );
            Assert.AreEqual( "a short thought", page.Key );
            Assert.AreEqual( "blogs", page.Parent.Name );
            Assert.AreEqual( "my blog", page.Parent.Key );
        }

        [Test]
        public void Blog_Child_Post_Page_With_Name_And_Action_Is_Selected()
        {
            string path = "/Blogs/My-Blog/Posts/A-Short-Thought/Comments/New";

            // Exercise
            SitePage page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "comments", page.Name );
            Assert.AreEqual( "new", page.Action );
        }

        [Test]
        public void Specific_Blog_By_Name()
        {
            string path = "/Blogs/My-Blog";

            // Exercise
            SitePage page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "blogs", page.Name );
            Assert.AreEqual( "detail", page.Action );
            Assert.AreEqual( "my blog", page.Key, StringComparison.InvariantCultureIgnoreCase );
        }

        [Test]
        public void All_Posts_Within_Blog()
        {
            string path = "/Blogs/My-Blog/Posts";

            // Exercise
            SitePage page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "posts", page.Name );
            Assert.AreEqual( "index", page.Action );
            Assert.AreEqual( "my blog", page.Parent.Key, StringComparison.InvariantCultureIgnoreCase );
        }

        [Test]
        public void Specific_Post_Within_Blog_With_Detail_Page()
        {
            string path = "/Blogs/My-Blog/Posts/An-Interesting-Story";

            // Exercise
            SitePage page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "blogs", page.Parent.Name );
            Assert.AreEqual( "my blog", page.Parent.Key, StringComparison.InvariantCultureIgnoreCase );
            Assert.AreEqual( "posts", page.Name );
            Assert.AreEqual( "detail", page.Action );
            Assert.AreEqual( "an interesting story", page.Key, StringComparison.InvariantCultureIgnoreCase );
        }
    }
}
