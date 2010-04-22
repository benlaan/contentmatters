using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Models;
using Laan.Persistence.Interfaces;
using Laan.Utilities.Xml;

namespace Laan.ContentMatters.Engine.Services
{
    public class DefinitionService : IDefinitionService
    {
        private const string CustomNamespace = "Laan.ContentMatters.Models.Custom";

        private string _appData;

        public DefinitionService( IMapper mapper )
        {
            _appData = mapper.MapPath( "~/App_Data" );
        }

        internal void EnsureDefinitionsHasParentProperty( List<ItemDefinition> definitions )
        {
            foreach ( ItemDefinition definition in definitions )
            {
                if ( definition.Parent == null )
                    continue;

                // If there is already a field that is named as per the Parent property, continue
                var def = definition.Fields.FirstOrDefault( fd => fd.Name == definition.Parent );
                if ( def != null )
                    continue;

                // Add a reference of type 'Parent', called 'Parent'
                // eg. public virtual Blog Blog { get; set; }
                definition.Fields.Add(
                    new FieldDefinition
                    {
                        Name = definition.Parent,
                        ReferenceType = definition.Parent,
                        FieldType = FieldType.Reference,
                        IsParent = true
                    }
                );
            }
        }

        public List<ItemDefinition> LoadDefinitions()
        {
            return XmlPersistence<List<ItemDefinition>>.LoadFromFile(
                Laan.Library.IO.Path.Combine( _appData, "Definitions", "definition.xml" )
            );
        }

        // TODO: This method *will* eventually load from a repository, for now, it
        //       is hardcoded!!!
        public void BuildTypesFromDefinitions()
        {
            List<ItemDefinition> defs = LoadDefinitions();
            EnsureDefinitionsHasParentProperty( defs );

            var typeConstructor = new TypeConstructor( CustomNamespace, Path.Combine( _appData, "Models" ) );
            typeConstructor.BuildAssemblyFromDefinitions( typeof( Item ), defs );
        }
    }
}
