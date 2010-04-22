using System;
using System.Collections.Generic;

namespace Laan.ContentMatters.Engine.Interfaces
{
    public interface IDataDictionary
    {
        string ExpandVariables( string text );
        void Clear();
        void Add( string key, object value );
        bool Remove( string key );
        object this[ string key ] { get; set; }
        bool TryGetValue( string key, out object value );
    }
}
