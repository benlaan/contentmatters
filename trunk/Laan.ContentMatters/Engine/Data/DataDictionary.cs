using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Engine.Data
{
    public class DataDictionary : IDataDictionary, IEnumerable<object>
    {
        private bool _hardFail;
        private Regex _regex;

        Dictionary<string, object> _data;

        /// <summary>
        /// Initializes a new instance of the DataDictionary class.
        /// </summary>
        public DataDictionary() : this( false )
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataDictionary class.
        /// </summary>
        /// <param name="hardFail"></param>
        public DataDictionary( bool hardFail )
        {
            _hardFail = hardFail;
            
            // compile the regex to find words and variables
            _regex = new Regex( @"\$?[_A-Za-z][\w\.]*|[\<\/\>\s\x21-\x23\x25-\x7D]+", RegexOptions.Compiled );
            _data = new Dictionary<string, object>();
        }

        private object GetData( string key )
        {
            string trimmedKey = key.TrimStart( '$' );

            object instance;
            if ( _data.TryGetValue( trimmedKey, out instance ) )
                return instance;

            int index = 0;
            string[] parts = trimmedKey.Split( new[] { '.' } );

            // Find 'root' object in dictionary, using the first part of the key
            if ( !_data.TryGetValue( parts[ index ], out instance ) )
            {
                if ( _hardFail )
                    throw new ArgumentException( String.Format( " Data not found for key: {0}", key ) );
                else
                    return key;
            }

            // Traverse
            while ( ++index < parts.Length )
            {
                if ( instance == null )
                    break;

                var type = instance.GetType();
                var prop = type.GetProperty( parts[ index ] );

                if ( prop != null )
                    instance = prop.GetValue( instance, null );
                else
                {
                    var method = type.GetMethod( parts[ index ].Trim( '(', ')' ) );
                    if ( method != null )
                        instance = method.Invoke( instance, null );
                    else
                    {
                        if ( _hardFail )
                            throw new ArgumentException( String.Format( "Property '{0}' not found on type '{1}' for key '{2}'", parts[ index ], type.Name, key ) );
                        
                        return key;
                    }
                }
            }

            return instance;
        }

        public void Clear()
        {
            _data.Clear();
        }

        public void Add( string key, object value )
        {
            if (_data.ContainsKey( key ))
                throw new ArgumentException( String.Format("key '{0}' is already in the data store, with a value of: {1}", key, _data[ key ]) );

            _data.Add( key, value );
        }

        public bool Remove( string key )
        {
            return _data.Remove( key );
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

        public string ExpandVariables( string text )
        {
            StringBuilder result = new StringBuilder();

            foreach ( Match match in _regex.Matches( text ) )
            {
                if ( !match.Success )
                    continue;

                if ( !match.Value.StartsWith( "$" ) )
                    result.Append( match.Value );
                else
                {
                    object instance;
                    if ( TryGetValue( match.Value, out instance ) )
                        result.Append( instance.ToString() );
                }
            }
            return result.ToString();
        }

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
