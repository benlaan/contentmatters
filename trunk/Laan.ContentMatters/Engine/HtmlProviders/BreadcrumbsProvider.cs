using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Linq;

using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Engine;
using Laan.ContentMatters.Configuration;

namespace Laan.ContentMatters.Engine.HtmlProviders
{
    public class BreadcrumbsProvider : XmlProvider
    {
        public class Page
        {
            public string Title { get; set; }
            public string Link { get; set; }
            
        }

        public override string ElementName
        {
            get { return "crumbs"; }
        }

        protected override void WriteXml( XmlReader reader, XmlWriter writer )
        {
            bool hideSingle = Convert.ToBoolean( reader.GetAttribute( "hideSingle" ) ?? "false" );
            string separator = reader.GetAttribute( "separator" ) ?? " | ";
            string dataName = GetRequiredAttribute( reader, "data" );

            object value;
            if ( !Data.TryGetValue( dataName, out value ) )
                throw new ArgumentNullException( String.Format( "{0} not found in data dictionary", dataName ) );

            SitePage page = value as SitePage;
            if ( page == null )
                throw new ArgumentNullException( String.Format( "{0} within the data dictionary isn't a SitePage", dataName ) );

            List<Page> results = new List<Page>();
            do
            {
                // Add 'Key' as a page
                if ( page.Key != null )
                    results.Add( new Page { Title = page.Key, Link = page.Link + "/" + page.Key } );

                // Add page itself
                results.Add( new Page { Title = page.Title, Link = page.Link } );
                
                page = page.Parent;
            }
            while ( page != null );

            // don't bother issuing any content if there is only one level of depth
            if ( hideSingle && ( results.Count == 1 ) )
                return;

            results = results.Reverse<Page>().ToList();

            SetFormatting( writer, Formatting.None );
            try
            {

                WriteElementWithOptionalClass( "div", reader, writer );
                Page last = results.Last();
                foreach ( var sitePage in results )
                {
                    writer.WriteStartElement( "a" );
                    writer.WriteAttributeString( "href", sitePage.Link );
                    writer.WriteValue( sitePage.Title );
                    writer.WriteFullEndElement();

                    if ( sitePage != last )
                        writer.WriteString( separator );
                }

                writer.WriteFullEndElement();
            }
            finally
            {
                ResetFormatting( writer );
            }
        }
    }
}
