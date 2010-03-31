using System;

using MbUnit.Framework;
using Laan.ContentMatters.Configuration;
using System.Text;
using Laan.ContentMatters.Tests;
using Laan.ContentMatters.Loaders;
using Laan.Utilities.Xml;
using Laan.ContentMatters.Provider;
using Laan.ContentMatters.Interfaces;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class ViewLoaderTest
    {
        private ViewLoader _viewLoader;

        [SetUp]
        public void Setup()
        {
            IXmlProvider[] providers = null;
            _viewLoader = new ViewLoader( providers, 2 );
        }

        [Test]
        public void Can_Output_Master_With_Two_Zones()
        {
            Page page = XmlPersistence<Page>.LoadFromFile( @"..\..\App_Data\Pages\test1.xml" );
            View view = _viewLoader.Load( page.Layout );
            Assert.IsNotNull( view );

            new[] 
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<html>",
                "  <head>",
                "    <title>Welcome</title>",
                "  </head>",
                "  <body>",
                "    <div>",
                "      <h1>Test</h1>",
                "      <div>main area!</div>",
                "      <br />",
                "    </div>",
                "    <div>",
                "      <div>sidebar!</div>",
                "    </div>",
                "  </body>",
                "</html>"
            }
            .Compare( view.Html );
        }

        [Test]
        public void Can_Output_Master_Zone_With_Nested_Zone()
        {
            Page page = XmlPersistence<Page>.LoadFromFile( @"..\..\App_Data\Pages\test2.xml" );
            View view = _viewLoader.Load( page.Layout );
            Assert.IsNotNull( view );

            new[] 
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<html>",
                "  <head>",
                "    <title>Test 2</title>",
                "  </head>",
                "  <body>",
                "    <div>",
                "      <h1>Test</h1>",
                "      <div>nested!</div>",
                "      <div>main area!</div>",
                "      <br />",
                "    </div>",
                "  </body>",
                "</html>"
            }
            .Compare( view.Html );
        }

        [Test]
        public void Can_Output_Master_Zone_With_Multiple_Nested_Zones()
        {
            Page page = XmlPersistence<Page>.LoadFromFile( @"..\..\App_Data\Pages\test3.xml" );
            View view = _viewLoader.Load( page.Layout );
            Assert.IsNotNull( view );

            new[] 
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<html>",
                "  <head>",
                "    <title>Test 2</title>",
                "  </head>",
                "  <body>",
                "    <div>",
                "      <h1>Test</h1>",
                "      <div>nested!</div>",
                "      <div>main area!</div>",
                "      <br />",
                "      <div>main area!</div>",
                "    </div>",
                "  </body>",
                "</html>"
            }
            .Compare( view.Html );
        }

    }
}
