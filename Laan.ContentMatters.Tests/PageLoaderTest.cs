using System;
using Laan.ContentMatters.Configuration;
using MbUnit.Framework;
using Laan.Utilities.Xml;
using Laan.ContentMatters.Engine;

namespace Laan.ContentMatters.Tests
{
    public class PageLoaderTest
    {
        private PageLoader _pageLoader;
        private PageConfiguration _pageConfiguration;

        public PageLoaderTest()
        {
        }

        private void LoadConfig( string[] xml )
        {
            var fullXml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                <pageConfiguration
                  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                  xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                >" + String.Join( "\n", xml ) + "</pageConfiguration>";

            _pageConfiguration = XmlPersistence<PageConfiguration>.LoadFromString( fullXml );
            _pageLoader = new PageLoader( _pageConfiguration );
        }

        [Test]
        [ExpectedException( typeof( PageNotFoundException ) )]
        public void Empty_Configuration_Raises_Exception()
        {
            string[] configXml = 
            { 
                "" 
            };

            // Setup
            LoadConfig( configXml );
            string path = "/";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );
        }

        [Test]
        [ExpectedException( typeof( TemplateNotFoundException ) )]
        public void Default_Page_Selected_But_Template_Not_Found()
        {
            string[] configXml = 
            { 
                "<pages>",
                "  <page name='home' template='home' layout='twoColumn' default='true'/>",
                "</pages>",
            };

            // Setup
            LoadConfig( configXml );
            string path = "/";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );
        }

        [Test]
        [ExpectedException( typeof( LayoutNotFoundException ) )]
        public void Default_Page_Selected_But_Layout_Not_Found()
        {
            string[] configXml = 
            { 
                "<pages>",
                "  <page name='home' template='home' layout='twoColumn' default='true'/>",
                "</pages>",
                "<templates>",
                "  <template name='home'/>",
                "</templates>",
            };

            // Setup
            LoadConfig( configXml );
            string path = "/";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );
        }

        [Test]
        public void Default_Page_Selected()
        {
            string[] configXml =
            { 
                "<pages>",
                "  <page name='home' template='home' layout='twoColumn' default='true'/>",
                "</pages>",
                "<templates>",
                "  <template name='home'/>",
                "</templates>",
                "<layouts>",
                "  <layout name='twoColumn'/>",
                "</layouts>",
            };

            // Setup
            LoadConfig( configXml );
            string path = "/";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "home", page.Name );
        }

        [Test]
        public void Default_Page_With_Named_Key_Selected()
        {
            string[] configXml = 
            { 
                "<pages>",
                "  <page name='home' template='home' layout='twoColumn' default='true'/>",
                "</pages>",
                "<templates>",
                "  <template name='home'/>",
                "</templates>",
                "<layouts>",
                "  <layout name='twoColumn'/>",
                "</layouts>",
            };

            // Setup
            LoadConfig( configXml );
            string path = "/Hello-World";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "home", page.Name );
            Assert.AreEqual( "hello world", page.Key );
        }

        [Test]
        public void Blog_Page_With_Name_Selected()
        {
            string[] configXml = 
            { 
                "<pages>",
                "  <page name='home' template='general' layout='twoColumn' default='true'/>",
                "  <page name='blog' template='general' layout='twoColumn'/>",
                "</pages>",
                "<templates>",
                "  <template name='general'/>",
                "</templates>",
                "<layouts>",
                "  <layout name='twoColumn'/>",
                "</layouts>",
            };

            // Setup
            LoadConfig( configXml );
            string path = "/Blog/My-Blog";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "blog", page.Name );
            Assert.AreEqual( "my blog", page.Key );
        }

        [Test]
        public void Blog_Child_Post_Page_Inherits_Parent_Template_And_Layout_Is_Selected()
        {
            string[] configXml = 
            { 
                "<pages>",
                "  <page name='home' template='general' layout='twoColumn' default='true'/>",
                "  <page name='blog' template='general' layout='twoColumn'>",
                "    <page name='post'/>",
                "  </page>",
                "</pages>",
                "<templates>",
                "  <template name='general'/>",
                "</templates>",
                "<layouts>",
                "  <layout name='twoColumn'/>",
                "</layouts>",
            };

            // Setup
            LoadConfig( configXml );
            string path = "/Blog/My-Blog/Post/A-Short-Thought/Edit";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "post", page.Name );
            Assert.AreEqual( "general", page.Template.Name );
            Assert.AreEqual( "twoColumn", page.Layout.Name );
        }

        [Test]
        public void Blog_Child_Post_Page_With_Name_Is_Selected()
        {
            string[] configXml = 
            { 
                "<pages>",
                "  <page name='home' template='general' layout='twoColumn' default='true'/>",
                "  <page name='blog' template='general' layout='twoColumn'>",
                "    <page name='post' layout='twoColumn'/>",
                "  </page>",
                "</pages>",
                "<templates>",
                "  <template name='general'/>",
                "</templates>",
                "<layouts>",
                "  <layout name='twoColumn'/>",
                "</layouts>",
            };

            // Setup
            LoadConfig( configXml );
            string path = "/Blog/My-Blog/Post/A-Short-Thought";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "post", page.Name );
            Assert.AreEqual( "a short thought", page.Key );
            Assert.AreEqual( "blog", page.Parent.Name );
            Assert.AreEqual( "my blog", page.Parent.Key );
        }

        [Test]
        public void Blog_Child_Post_Page_With_Name_And_Action_With_Overriden_Layout_Is_Selected()
        {
            string[] configXml = 
            { 
                "<pages>",
                "  <page name='home' template='general' layout='twoColumn' default='true'/>",
                "  <page name='blog' template='general' layout='twoColumn'>",
                "    <page name='post'>",
                "      <page name='comment' layout='oneColumn'/>",
                "    </page>",
                "  </page>",
                "</pages>",
                "<templates>",
                "  <template name='general'/>",
                "</templates>",
                "<layouts>",
                "  <layout name='oneColumn'/>",
                "  <layout name='twoColumn'/>",
                "</layouts>",
            };

            // Setup
            LoadConfig( configXml );
            string path = "/Blog/My-Blog/Post/A-Short-Thought/Comment/New";

            // Exercise
            Page page = _pageLoader.GetPageFromPath( path );

            Assert.IsNotNull( page );
            Assert.AreEqual( "comment", page.Name );
            Assert.AreEqual( "new", page.Action );
            Assert.AreEqual( "oneColumn", page.Layout.Name );
        }

        //[Test]
        //public void Verify_Links()
        //{
        //    string[] configXml = { 
        //        "<pages>",
        //        "  <page name='home' template='general' layout='twoColumn' default='true'/>",
        //        "  <page name='blog' template='general' layout='twoColumn'>",
        //        "    <page name='post'>",
        //        "      <page name='comment' layout='oneColumn'/>",
        //        "    </page>",
        //        "  </page>",
        //        "</pages>",
        //        "<templates>",
        //        "  <template name='general'/>",
        //        "</templates>",
        //        "<layouts>",
        //        "  <layout name='oneColumn'/>",
        //        "  <layout name='twoColumn'/>",
        //        "</layouts>",
        //    };

        //    // Setup
        //    LoadConfig( configXml );
        //    string path = "/Blog/My-Blog/Post/A-Short-Thought/Comment/New";

        //    // Exercise
        //    Page page = _pageLoader.GetPageFromPath( path );

        //    Assert.IsNotNull( page );
        //    Assert.AreEqual( "/Blog/My-Blog/Post/A-Short-Thought/Comment", page.Link );
        //}
    
    }
}
