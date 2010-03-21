using System;
using System.Collections.Generic;

using Laan.ContentMatters.Configuration;
using Laan.Utilities.Xml;

using MbUnit.Framework;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class SitePageTests
    {
        private SitePage _homePage;
        private SitePage _blogPage;
        private SitePage _newsPages;
        private SitePage _postPage;
        private SitePage _commentPage;

        [SetUp]
        public void Setup()
        {
            _homePage = new SitePage()    { Name = "home" };
            _newsPages = new SitePage()   { Name = "news",     Parent = _homePage, Folder = "News" };
            _blogPage = new SitePage()    { Name = "blogs",    Parent = _homePage, Folder = "Blogs" };
            _postPage = new SitePage()    { Name = "posts",    Parent = _blogPage, Folder = "Posts" };
            _commentPage = new SitePage() { Name = "comments", Parent = _postPage };           
        }
        
        [Test]
        public void Page_Folder_Is_Inherited_From_Parent_If_Child_Page_Has_None_Specified()
        {
            Assert.AreEqual( @"News\news", _newsPages.FileName, StringComparison.OrdinalIgnoreCase );
        }

        [Test]
        public void Page_Folder_Is_Inherited_From_Multiple_Ancestors()
        {
            Assert.AreEqual( @"Blogs\Posts\comments", _commentPage.FileName, StringComparison.OrdinalIgnoreCase );
        }
    }
}
