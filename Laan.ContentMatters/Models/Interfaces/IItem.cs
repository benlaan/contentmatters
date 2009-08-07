using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Models
{
    public interface IItem
    {
//        ItemDefinition ItemDefinition { get; }
        string TypeName { get; }
        string Description { get; }
        string Title { get; }
    }

    public interface IItemList<T> : IItem
    {
        
    }
}