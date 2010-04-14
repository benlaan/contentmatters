using System;
using System.Collections.Generic;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Loaders;
using Laan.Utilities.Xml;
using Laan.Persistence.Interfaces;

using MbUnit.Framework;

using Rhino.Mocks;
using Laan.ContentMatters.Engine.Data;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class DataDictionaryTest
    {
        private MockRepository mock = new MockRepository();

        [Test]
        public void Can_Access_Simple_Object()
        {
            DataDictionary data = new DataDictionary( true );
            data.Add( "person", new { Name = "Ben" } );

            Assert.IsNotNull( data[ "person" ] );
        }

        [Test]
        public void Can_Access_Simple_Object_Property()
        {
            DataDictionary data = new DataDictionary( true );
            data.Add( "person", new { Name = "Ben" } );

            Assert.AreEqual( "Ben", data[ "person.Name" ] );
        }

        [Test]
        public void Can_Access_Object_Property_Within_Nested_Object()
        {
            DataDictionary data = new DataDictionary( true );
            data.Add( 
                "person", 
                new { 
                    Name = "Ben", 
                    Address = new { State = "SA" } 
                } 
            );

            Assert.AreEqual( "SA", data[ "person.Address.State" ] );
        }
    }
}
