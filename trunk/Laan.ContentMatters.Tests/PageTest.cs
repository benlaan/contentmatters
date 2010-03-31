using System;
using MbUnit.Framework;
using Laan.ContentMatters.Configuration;
using Laan.Utilities.Xml;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class PageTest
    {
        [Test]
        public void Can_Open_Simple_Page_With_Data()
        {
            var xml = new[] 
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
                "<page name=\"home\">",
                "  <data name=\"home.news\" type=\"news\" select=\"all\" order=\"date desc\" top=\"10\"/>",
                "  <data name=\"home.photos\" type=\"photo\" select=\"random\" order=\"date\" top=\"10\"/>",
                "</page>"
            };

            Page page = XmlPersistence<Page>.LoadFromString( String.Join( Environment.NewLine, xml ) );

            Assert.AreEqual( "home", page.Name );
            Assert.AreEqual( 2, page.DataSources.Count );
                        
            DataSource homeData = page.DataSources[0];
            Assert.AreEqual( "home.news", homeData.Name );
            Assert.AreEqual( "news", homeData.Type );
            Assert.AreEqual( SelectionMode.All, homeData.Select );
            Assert.AreEqual( "date desc", homeData.Order );
            Assert.AreEqual( 10, homeData.Top );
        }

        [Test]
        public void Can_Open_Simple_Page_With_View_Within_Main_Zone()
        {
            var xml = new[] 
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
                "<page name=\"home\">",
                "  <layout page=\"site\\master\">",
                "    <view page=\"body\" zone=\"main\" />",
                "    <view page=\"sidebar\" zone=\"sidebar\" />",
                "  </layout>",
                "</page>"
            };

            Page page = XmlPersistence<Page>.LoadFromString( String.Join( Environment.NewLine, xml ) );

            Assert.IsNotNull( page.Layout );
            Assert.AreEqual( @"site\master", page.Layout.Page );
            Assert.AreEqual( 2, page.Layout.Views.Count );
            Assert.AreEqual( "body", page.Layout.Views[ 0 ].Page );
            Assert.AreEqual( "main", page.Layout.Views[ 0 ].Zone );
        }

        [Test]
        public void Can_Open_Simple_Page_With_View_With_Nested_Layouts()
        {
            var xml = new[] 
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
                "<page name=\"home\">",
                "  <layout page=\"site\\master\">",
                "    <view zone=\"main\" >",
                "      <layout page=\"vert-split\">",
                "        <view page=\"home\\welcome\" zone=\"top\" />",
                "      </layout>",
                "    </view>",
                "    <view zone=\"header\" page=\"site\\header\" />",
                "  </layout>",
                "</page>"
            };

            Page page = XmlPersistence<Page>.LoadFromString( String.Join( Environment.NewLine, xml ) );

            Assert.IsNotNull( page.Layout );
            Assert.AreEqual( @"site\master", page.Layout.Page );
            Assert.AreEqual( 2, page.Layout.Views.Count );
            
            PageView mainView = page.Layout.Views[0];
            Assert.AreEqual( null, mainView.Page );
            Assert.AreEqual( "main", mainView.Zone );

            Assert.AreEqual( "vert-split", mainView.Layout.Page );
            Assert.AreEqual( 1, mainView.Layout.Views.Count );
            Assert.AreEqual( "home\\welcome", mainView.Layout.Views[ 0 ].Page );
            Assert.AreEqual( "top", mainView.Layout.Views[ 0 ].Zone );

            PageView headerView = page.Layout.Views[1];
            Assert.AreEqual( "site\\header", headerView.Page );
            Assert.AreEqual( "header", headerView.Zone );
        }
    }
}
