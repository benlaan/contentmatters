using System;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Loaders;
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
    public class WikipediaProviderTest : BaseXmlProviderTest<WikipediaProvider>
    {
        /// <summary>
        /// Initializes a new instance of the WikipediaProviderTest class.
        /// </summary>
        public WikipediaProviderTest() : base( "wiki" )
        {
            
        }

        [Test]
        [ExpectedArgumentException]
        public void Can_Render_Empty_List()
        {
            // Setup
            var data = new DataDictionary( true );
                        
            string input = "<wiki></wiki>";
            String output = GetXml( input, data );
        }

        [Test]
        public void Can_Render_With_Search_Term()
        {
            // Setup
            var items = new string[0];
            var data = new DataDictionary( true ) { { "items", items } };

            string input = "<wiki>Words</wiki>";
            String output = GetXml( input, data );

            new[] 
            { 
                "<a href=\"http://www.wikipedia.org/wiki/Words\" title=\"Visit Wikipedia for information about Words\">Words</a>" 
            }
            .Compare( output );            
        }

        [Test]
        public void Can_Render_With_Search_Term_With_Spaces()
        {
            // Setup
            var items = new string[ 0 ];
            var data = new DataDictionary( true ) { { "items", items } };

            string input = "<wiki>Many Words</wiki>";
            String output = GetXml( input, data );

            new[] 
            { 
                "<a href=\"http://www.wikipedia.org/wiki/Many_Words\" title=\"Visit Wikipedia for information about Many Words\">Many Words</a>" 
            }
            .Compare( output );
        }

        [Test]
        public void Can_Render_With_Specified_Lookup_Href()
        {
            // Setup
            var items = new string[ 0 ];
            var data = new DataDictionary( true ) { { "items", items } };

            string input = "<wiki href=\"Intelligibility_(philosophy)\">Many Words</wiki>";
            String output = GetXml( input, data );

            new[] 
            { 
                "<a href=\"http://www.wikipedia.org/wiki/Intelligibility_(philosophy)\" title=\"Visit Wikipedia for information about Many Words\">Many Words</a>" 
            }
            .Compare( output );
        }
    }
}
