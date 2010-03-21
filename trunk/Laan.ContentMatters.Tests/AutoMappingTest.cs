using System;

using MbUnit.Framework;
using Rhino.Mocks;
using Castle.Core.Configuration;
using Laan.Persistence.AutoMapping;
using Laan.ContentMatters.Models;

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
            var autoMapper = new AutoMappingBuilder( null );

            IConfiguration config = _mock.Stub<IConfiguration>();

            using (_mock.Record())
            {
                var children = new ConfigurationCollection();

                var assemblies = new MutableConfiguration("assemblies");
                assemblies.CreateChild("assemblies", "Laan.ContentMatters");
                children.Add(assemblies);

                var options = new MutableConfiguration("options");
                //options.CreateChild("options");
                options.Children.Add(new MutableConfiguration("baseType", typeof(Item).AssemblyQualifiedName));
                options.Children.Add(new MutableConfiguration("filterType", typeof(IIdentifiable).AssemblyQualifiedName));
                children.Add(options);

                var settings = new MutableConfiguration("settings");
                //settings.CreateChild("options");
                settings.Children.Add(new MutableConfiguration("baseType", typeof(Item).AssemblyQualifiedName));
                settings.Children.Add(new MutableConfiguration("filterType", typeof(IIdentifiable).AssemblyQualifiedName));
                children.Add(settings);

                Expect.Call(config.Children).Return(children).Repeat.Any();
            }

            using (_mock.Playback())
            {
                autoMapper.GetConfiguration(config);
            }
        }
    }
}



