using System;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.Utilities.Xml;
using Laan.Persistence.Interfaces;

using MbUnit.Framework;

using Rhino.Mocks;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using Laan.ContentMatters.Engine.HtmlProviders;
using Laan.ContentMatters.Engine.Data;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class BreadcrumbsProviderTest : BaseXmlProviderTest<BreadcrumbsProvider>
    {
        /// <summary>
        /// Initializes a new instance of the BreadcrumbsProviderTest class.
        /// </summary>
        public BreadcrumbsProviderTest() : base( "crumbs" )
        {

        }

        [Test]
        [ExpectedArgumentException()]
        public void Can_Render_Empty_List()
        {
            // Setup
            var data = new DataDictionary( true );

            string input = "<crumbs />";
            String output = GetXml( input, data );
        }

        [Test]
        public void Can_Render_With_No_Child_Pages()
        {
            // Setup
            var page = new SitePage { Page = new Page { Name = "Blogs" } };
            var data = new DataDictionary( true ) { { "page", page } };


            string input = "<crumbs data=\"$page\"/>";
            String output = GetXml( input, data );

            Assert.AreEqual( "<div><a href=\"/Blogs\">Blogs</a></div>", output );
        }

        [Test]
        public void Can_Render_Empty_Due_To_Hiding_Single_Page_Is_False()
        {
            // Setup
            var page = new SitePage { Page = new Page { Name = "Blogs" } };
            var data = new DataDictionary( true ) { { "page", page } };


            string input = "<crumbs data=\"$page\" hideSingle=\"false\"/>";
            String output = GetXml( input, data );

            Assert.AreEqual( "<div><a href=\"/Blogs\">Blogs</a></div>", output );
        }

        [Test]
        public void Can_Render_Empty_Due_To_Hiding_Single_Page_Is_True()
        {
            // Setup
            var page = new SitePage { Page = new Page { Name = "Blogs" } };
            var data = new DataDictionary( true ) { { "page", page } };

            string input = "<crumbs data=\"$page\" hideSingle=\"true\"/>";
            String output = GetXml( input, data );

            Assert.AreEqual( "", output );
        }

        [Test]
        public void Can_Render_With_Child_Page()
        {
            // Setup
            var blogs = new SitePage { Page = new Page { Name = "Blogs" } };
            var page = new SitePage { Page = new Page { Name = "My Blog" }, Parent = blogs };

            var data = new DataDictionary( true ) { { "page", page } };

            string input = "<crumbs data=\"$page\"/>";
            String output = GetXml( input, data );

            Assert.AreEqual( "<div><a href=\"/Blogs\">Blogs</a> | <a href=\"/Blogs/My Blog\">My Blog</a></div>", output );
        }

        [Test]
        public void Can_Render_With_Child_Page_With_Hide_Single_True()
        {
            // Setup
            var blogs = new SitePage { Page = new Page { Name = "Blogs" } };
            var page = new SitePage { Page = new Page { Name = "My Blog" }, Parent = blogs };

            var data = new DataDictionary( true ) { { "page", page } };

            string input = "<crumbs data=\"$page\" hideSingle=\"true\"/>";
            String output = GetXml( input, data );

            Assert.AreEqual( "<div><a href=\"/Blogs\">Blogs</a> | <a href=\"/Blogs/My Blog\">My Blog</a></div>", output );
        }

        [Test]
        public void Can_Render_With_Nested_Child_Pages()
        {
            // Setup
            var blogs = new SitePage { Page = new Page { Name = "Blogs" } };
            var posts = new SitePage { Page = new Page { Name = "My Blog" }, Parent = blogs };
            var page  = new SitePage { Page = new Page { Name = "Posts" }, Parent = posts };

            var data = new DataDictionary( true ) { { "page", page } };

            string input = "<crumbs data=\"$page\"/>";
            String output = GetXml( input, data );

            Assert.AreEqual( "<div><a href=\"/Blogs\">Blogs</a> | <a href=\"/Blogs/My Blog\">My Blog</a> | <a href=\"/Blogs/My Blog/Posts\">Posts</a></div>", output );
        }

        public void Can_Render_With_Specified_Separator()
        {
            // Setup
            var blogs = new SitePage { Name = "Blogs", Parent = null, Page = new Page { Key = "My Blog" } };
            var page  = new SitePage { Name = "Posts", Parent = blogs };

            var data = new DataDictionary( true ) { { "page", page } };

            string input = "<crumbs data=\"$page\" separator=\" > \" />";
            String output = GetXml( input, data );

            Assert.AreEqual( "<div><a href=\"/Blogs\">Blogs</a> &gt; <a href=\"/Blogs/My Blog\">My Blog</a> &gt; <a href=\"/Blogs/My Blog/Posts\">Posts</a></div>", output );
        }

    }
}
