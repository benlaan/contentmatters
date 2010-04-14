using System;
using System.IO;
using System.Xml;

using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Engine.HtmlProviders;
using MbUnit.Framework;

namespace Laan.ContentMatters.Tests
{
    public abstract class BaseXmlProviderTest<T> : BaseTestFixture where T : IXmlProvider, new()
    {
        private string _elementName;

        /// <summary>
        /// Initializes a new instance of the BaseXmlProviderTest class.
        /// </summary>
        /// <param name="elementName"></param>
        public BaseXmlProviderTest( string elementName )
        {
            _elementName = elementName;
        }

        protected String GetXml( string inputXml, IDataDictionary data )
        {
            T provider = new T();
            using ( StringReader sr = new StringReader( inputXml ) )
            {
                using ( XmlReader reader = new XmlTextReader( sr ) )
                {
                    reader.Read();
                    using ( XmlReader result = provider.ReplaceElement( reader, data ) )
                    {
                        result.Read();
                        return result.ReadOuterXml();
                    }
                }
            }
        }

        [Test]
        public void Ensure_Correct_ElementName_Is_Provided()
        {
            var provider = new T();
            Assert.AreEqual( _elementName, provider.ElementName );
        }
    }
}
