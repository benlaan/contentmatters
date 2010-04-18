using System;
using Laan.Persistence.Interfaces;
using System.Collections.Generic;
using Laan.ContentMatters.Models;

namespace Laan.ContentMatters.Engine.Interfaces
{
    public interface IDefinitionService
    {
        List<ItemDefinition> LoadDefinitions();
        void BuildTypesFromDefinitions();
    }
}
