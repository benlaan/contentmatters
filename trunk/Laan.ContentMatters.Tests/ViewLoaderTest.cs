using System;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Engine.Data;
using Laan.ContentMatters.Loaders;
using Laan.ContentMatters.Tests;
using Laan.Utilities.Xml;
using Laan.Persistence.Interfaces;

using MbUnit.Framework;

using Rhino.Mocks;

namespace Laan.ContentMatters.Tests
{

    [TestFixture]
    public class ViewLoaderTest
    {
        private View GetView( string pageName)
        {
            MockRepository mock = new MockRepository();

            IDataDictionary data = new DataDictionary();
            IMapper mapper = mock.DynamicMock<IMapper>();
            IDataProvider dataProvider = mock.Stub<IDataProvider>();

            using ( mock.Record() )
            {
                Expect.Call( mapper.MapPath( "~/App_Data" ) ).Return( @"..\..\App_Data" ).Repeat.Any();
                Expect.Call( dataProvider.Build( null ) ).IgnoreArguments().Return( null ).Repeat.Any();
            }

            View view;
            using ( mock.Playback() )
            {
                ViewLoader viewLoader = new ViewLoader( mapper, dataProvider, data, 2 );
                Page page = XmlPersistence<Page>.LoadFromFile( String.Format( @"..\..\App_Data\Pages\{0}", pageName) );
                view = viewLoader.Load( page );
            }
            Assert.IsNotNull( view );
            return view;
        }

        [Test]
        public void Can_Output_Master_With_Two_Zones()
        {
            View view = GetView( @"test1.xml" );

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
                "      <div id=\"another\">",
                "        <div id=\"inside\" />",
                "      </div>",
                "      <br />",
                "      <div id=\"another\">",
                "        <div id=\"inside\" />",
                "      </div>",
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
            View view = GetView( @"test2.xml" );

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
            View view = GetView( @"test3.xml" );

            new[] 
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<html>",
                "  <head>",
                "    <title>Test 3</title>",
                "  </head>",
                "  <body>",
                "    <div>",
                "      <h1>Test 3</h1>",
                "      <div>nested!</div>",
                "      <div>main area!</div>",
                "      <br />",
                "      <div>main area!</div>",
                "      <div id=\"another\">",
                "        <div id=\"inside\" />",
                "      </div>",
                "    </div>",
                "  </body>",
                "</html>"
            }
            .Compare( view.Html );
        }

    }
}
