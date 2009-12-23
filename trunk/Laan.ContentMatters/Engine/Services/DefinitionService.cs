using System;
using System.Collections.Generic;
using System.IO;

using Laan.ContentMatters.Models;
using Laan.Persistence.Interfaces;
using Laan.Utilities.Xml;
using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Engine.Services
{
    public class DefinitionService : IDefinitionService
    {
        private const string CustomNamespace = "Laan.ContentMatters.Models.Custom";

        private IMapper _mapper;

        public DefinitionService( IMapper mapper )
        {
            _mapper = mapper;
        }

        /// <summary>
        /// This method *will* eventually load from a repository, for now, it
        /// is hardcoded!!!
        /// </summary>
        public void LoadItemDefinitions()
        {
            string appData = _mapper.MapPath( "~/App_Data" );

            var defs = XmlPersistence<List<ItemDefinition>>.LoadFromFile(
                Laan.Library.IO.Path.Combine( appData, "Definitions", "definition.xml" )
            );

            var typeConstructor = new TypeConstructor( CustomNamespace, Path.Combine( appData, "Models" ) );
            typeConstructor.BuildAssemblyFromDefintions( typeof( Item ), defs );

            //HttpRuntime.UnloadAppDomain();
        }
    }
}
