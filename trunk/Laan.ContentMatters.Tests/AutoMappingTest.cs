using System;

using MbUnit.Framework;

using Rhino.Mocks;

using Castle.Core.Configuration;
using Laan.Persistence.AutoMapping;
using Laan.ContentMatters.Engine;
using Laan.Persistence.Interfaces;
using Castle.MicroKernel;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class AutoMappingTest : BaseTestFixture
    {
        [Test]
        public void Test_Can_Create_AutoMap_Configurator()
        {
            var kernel = Dynamic<IKernel>();
            var mapper = Dynamic<IMapper>();
            var autoMapper = new AutoMappingBuilder( kernel );

            IConfiguration config = _mock.Stub<IConfiguration>();

            using ( _mock.Record() )
            {
                var children = new ConfigurationCollection();
                var assemblies = new MutableConfiguration( "assemblies" );
                
                assemblies.CreateChild( "assembly", "Laan.ContentMatters" );
                children.Add( assemblies );
                
                Expect.Call( config.Children ).Return( children );
                Expect.Call( kernel.Resolve<IMapper>() ).Return( mapper );
            }

            using ( _mock.Playback() )
            {
                autoMapper.GetConfiguration( config );
            }
        }
    }
}
