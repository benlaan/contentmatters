using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace XmlTest
{
    class Program
    {
        private static void WriteElement( XmlReader reader, XmlTextWriter writer )
        {
            writer.WriteStartElement( reader.Name );
            writer.WriteAttributes( reader, false );

            if ( reader.IsEmptyElement )
                writer.WriteEndElement();
        }

        private static void ProcessNodes( XmlReader reader, XmlTextWriter writer )
        {
            while ( !reader.EOF )
            {
                reader.Read();
                switch ( reader.NodeType )
                {
                    case XmlNodeType.Element:
                        WriteElement( reader, writer );
                        
                        break;

                    case XmlNodeType.EndElement:
                        writer.WriteEndElement();
                        break;

                    case XmlNodeType.Text:
                        writer.WriteRaw( reader.Value );
                        break;
                }
            }
        }

        static void Main( string[] args )
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                using ( XmlReader reader = XmlNodeReader.Create( "in.xml", settings ) )
                {
                    using ( XmlTextWriter writer = new XmlTextWriter( "out.xml", Encoding.UTF8 ) )
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();
                        
                        ProcessNodes( reader, writer );
                    }
                }

                Console.Write( File.ReadAllText( "out.xml" ) );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex );
            }
            Console.ReadKey();
        }
    }
}
