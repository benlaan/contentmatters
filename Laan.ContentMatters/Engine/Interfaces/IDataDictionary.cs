using System;
using System.Collections.Generic;

namespace Laan.ContentMatters.Engine.Interfaces
{
    public interface IDataDictionary
    {
        string UnwrapVariables( string text );
        void Clear();
        void Add( string key, object value );
        bool Remove( string key );
        ICollection<string> Keys { get; }
        ICollection<object> Values { get; }
        object this[ string key ] { get; set; }
        bool TryGetValue( string key, out object value );
    }
}
