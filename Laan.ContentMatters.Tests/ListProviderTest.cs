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
    public class ListProviderTest : BaseXmlProviderTest<ListProvider>
    {
        /// <summary>
        /// Initializes a new instance of the ListProviderTest class.
        /// </summary>
        public ListProviderTest() : base( "list" )
        {
             
        }

        [Test]
        public void Can_Render_Empty_List()
        {
            // Setup
            var days = new string[0];
            var data = new DataDictionary( true ) { { "days", days } };
                        
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
            var data = new DataDictionary( true ) { { "items", items } };
                        
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
            var data = new DataDictionary( true ) { { "days", days } };
                        
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

        [Test]
        public void Can_Render_List_Of_Items_With_Template()
        {
            // Setup
            var links = new[] 
            { 
                new { Text = "Home", Link = "/" },
                new { Text = "Blog", Link = "/Blogs/Ben" },
                new { Text = "Code", Link = "/Code/List" },
                new { Text = "About", Link = "/About/Ben" },
            };

            var data = new DataDictionary( true ) { { "links", links } };

            string input =
                @"<list data='links'>
                    <detail><a href='$item.Link'>$item.Text</a></detail>
                  </list>";
            String output = GetXml( input, data );

            new[] 
            { 
                "<ul>", 
                "  <li><a href=\"/\">Home</a></li>",
                "  <li><a href=\"/Blogs/Ben\">Blog</a></li>",
                "  <li><a href=\"/Code/List\">Code</a></li>",
                "  <li><a href=\"/About/Ben\">About</a></li>",
                "</ul>" 
            
            }.Compare( output );
        }

    }
}
