using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Models
{
    public interface IItem
    {
    }

    public interface IItemList<T> : IItem
    {
        
    }
}