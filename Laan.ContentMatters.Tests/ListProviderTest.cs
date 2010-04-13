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
    public class ListProviderTest
    {
        private String GetXml( string inputXml, IDataDictionary data )
        {
            var listProvider = new ListProvider();

            using ( StringReader sr = new StringReader( inputXml ) )
            using ( XmlReader reader = new XmlTextReader( sr ) )
            {
                reader.Read();
                using ( XmlReader result = listProvider.ReplaceElement( reader, data ) )
                {
                    result.Read();
                    return result.ReadOuterXml();
                }
            }
        }

        [Test]
        public void Ensure_Correct_ElementName_Is_Provided()
        {
            var listProvider = new ListProvider();
            Assert.AreEqual( "list", listProvider.ElementName );
        }

        [Test]
        public void Can_Render_Empty_List()
        {
            // Setup
            var days = new string[0];
            var data = new DataDictionary() { { "days", days } };
                        
            string input = "<list data=\"days\"/>";
            String output = GetXml( input, data );

            new[] 
            { 
                "<ul>", 
                "</ul>" 
            
            }.Compare( output );      
        }

        [Test]
        public void Can_Render_List_With_Specified_Class()
        {
            // Setup
            var items = new string[0];
            var data = new DataDictionary { { "items", items } };
                        
            string input = "<list data=\"items\" class=\"menu\"/>";
            String output = GetXml( input, data );

            new[] 
            { 
                "<ul class=\"menu\">", 
                "</ul>" 
            
            }.Compare( output );            
        }

        [Test]
        public void Can_Render_List_Of_Strings()
        {
            // Setup
            var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            var data = new DataDictionary { { "days", days } };
                        
            string input = "<list data=\"days\"/>";
            String output = GetXml( input, data );

            new[] 
            { 
                "<ul>", 
                "  <li>Monday</li>",
                "  <li>Tuesday</li>", 
                "  <li>Wednesday</li>", 
                "  <li>Thursday</li>", 
                "  <li>Friday</li>", 
                "</ul>" 
            
            }.Compare( output );
        }

    }
}
