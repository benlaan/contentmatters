using System;

using MbUnit.Framework;
using Laan.ContentMatters.Configuration;
using System.Text;
using Laan.ContentMatters.Tests;
using Laan.ContentMatters.Loaders;
using Laan.Utilities.Xml;
using Laan.ContentMatters.Interfaces;
using Laan.Persistence.Interfaces;
using Rhino.Mocks;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class IfProviderTest
    {
        private MockRepository _mock;
        private ViewLoader _viewLoader;

        [SetUp]
        public void Setup()
        {
            _mock = new MockRepository();

            IXmlProvider[] providers = null;
            IMapper mapper = _mock.DynamicMock<IMapper>();
                        
            using ( _mock.Record() )
            {
                Expect.Call( mapper.MapPath( "~/App_Data" ) ).Return( @"..\..\App_Data" ).Repeat.Any();
            }

            using ( _mock.Playback() )
            {
                _viewLoader = new ViewLoader( mapper, providers, 2 );
            }
        }

        [Test]
        public void Can_Output_Xml_With_NVelocity_Directives()
        {
            Page page = XmlPersistence<Page>.LoadFromFile( @"..\..\App_Data\Pages\test4.xml" );
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
                "      <div>directive!</div>",
                "#if ($a == 1)",
                "        <h1>Hello</h1>",
                "#else",
                "        <h1>Good Bye</h1>",
                "#end",
                "    </div>",
                "  </body>",
                "</html>"
            }
            .Compare( view.Html );
        }

    }
}
