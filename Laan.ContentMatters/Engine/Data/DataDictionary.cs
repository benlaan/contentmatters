using System;
using System.Collections.Generic;

using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Engine.Data
{
    public class DataDictionary : IDataDictionary, IEnumerable<object>
    {
        Dictionary<string, object> _data;

        public DataDictionary()
        {
            _data = new Dictionary<string, object>();
        }

        private object GetData( string key )
        {
            object instance;
            if ( _data.TryGetValue( key, out instance ) )
                return instance;

            string[] parts = key.Split( new[] { '.' } );

            int index = 0;
            if ( !_data.TryGetValue( parts[ index ], out instance ) )
                throw new ArgumentException( String.Format(" Data not found for key: {0}", key ) );

            index++;
            while ( index < parts.Length )
            {
                Type t = instance.GetType();
                var prop = t.GetProperty( parts[ index ] );
                if (prop == null)
                    throw new ArgumentException( String.Format("Property {0} not found on type {1}", parts[index], t.Name) );

                instance = prop.GetValue( instance, null );
                index++;
            }

            return instance;
        }

        #region IDictionary<string,object> Members

        public void Add( string key, object value )
        {
            _data.Add( key, value );
        }

        public bool Remove( string key )
        {
            return _data.Remove( key );
        }

        public ICollection<string> Keys
        {
            get { return _data.Keys; }
        }
        
        public ICollection<object> Values
        {
            get { return _data.Values; }
        }
        
        public object this[ string key ]
        {
            get { return GetData( key ); }
            set { _data[ key ] = value; }
        }

        public bool TryGetValue( string key, out object value )
        {
            value = GetData(key);
            return ( value != null );
        }

        #endregion

        #region IEnumerable<object> Members

        //public IEnumerator<object> GetEnumerator()
        //{
        //    return _data.GetEnumerator();
        //}

        #endregion

        #region IEnumerable Members

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region IEnumerable<object> Members

        public IEnumerator<object> GetEnumerator()
        {
            return _data.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        #endregion
    }
}
