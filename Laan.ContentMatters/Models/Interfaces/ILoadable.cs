using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.ContentMatters.Models.Services;

namespace Laan.ContentMatters.Models.Interfaces
{
    interface ILoadable<T> where T : IItem
    {
        ItemList<T> All( int? id );
        T One( int id );
    }
}