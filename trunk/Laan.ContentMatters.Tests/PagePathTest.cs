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
            Page page = _pageLoader.GetPageFromPath( path );
            Assert.AreEqual(page.Name, "home");
        }

        [Test]
        public void Blog_Page_With_Name_Selected()
        {
            string path = "/Blogs/My-Blog";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "blogs", page.Name );
            Assert.AreEqual( "my blog", page.Key );
        }

        [Test]
        public void Blog_Child_Post_Page_With_Verb()
        {
            string path = "/Blogs/My-Blog/Posts/A-Short-Thought/Edit";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "posts", page.Name );
        }

        [Test]
        public void Blog_Child_Post_Page_With_Name()
        {
            string path = "/Blogs/My-Blog/Posts/A-Short-Thought";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "posts", page.Name );
            Assert.AreEqual( "a short thought", page.Key );
            Assert.AreEqual( "blogs", page.Parent.Name );
            Assert.AreEqual( "my blog", page.Parent.Key );
        }

        [Test]
        public void Blog_Child_Post_Page_With_Name_And_Action_Is_Selected()
        {
            string path = "/Blogs/My-Blog/Posts/A-Short-Thought/Comments/New";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "comments", page.Name );
            Assert.AreEqual( "new", page.Action );
        }

        [Test]
        public void Specific_Blog_By_Name()
        {
            string path = "/Blogs/My-Blog";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "blogs", page.Name );
            Assert.AreEqual( "my blog", page.Key, StringComparison.InvariantCultureIgnoreCase );
        }

        [Test]
        public void All_Posts_Within_Blog()
        {
            string path = "/Blogs/My-Blog/Posts";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "posts", page.Name );
            Assert.AreEqual( "my blog", page.Parent.Key, StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void Specific_Post_Within_Blog_With_Detail_Page()
        {
            string path = "/Blogs/My-Blog/Posts/An-Interesting-Story";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "blogs", page.Parent.Name );
            Assert.AreEqual( "my blog", page.Parent.Key, StringComparison.InvariantCultureIgnoreCase );
            Assert.AreEqual( "posts", page.Name );
            Assert.AreEqual( "an interesting story", page.Key, StringComparison.InvariantCultureIgnoreCase );
        }
    }
}
