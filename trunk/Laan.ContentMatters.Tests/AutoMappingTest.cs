using System;

using MbUnit.Framework;
using Laan.ContentMatters.Utilities;
using Rhino.Mocks;
using Castle.Core.Configuration;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class AutoMappingTest
    {
        private MockRepository _mock;

        [SetUp]
        public void Setup()
        {
            _mock = new MockRepository();
        }

        [Test]
        public void Test_Can_Create_AutoMap_Configurator()
        {
            var autoMapper = new AutoMappingBuilder();

            IConfiguration config = _mock.Stub<IConfiguration>();

            using ( _mock.Record() )
            {
                var children = new ConfigurationCollection();
                var assemblies = new MutableConfiguration( "assemblies" );
                
                assemblies.CreateChild( "assembly", "Laan.ContentMatters" );
                children.Add( assemblies );
                
                Expect.Call( config.Children ).Return( children );
            }

            using ( _mock.Playback() )
            {
                autoMapper.GetConfiguration( config );
            }
        }
    }
}



