using System;
using System.IO;
using System.Linq;

namespace Laan.Library.IO
{
    public static class Path
    {
        public static string Combine( params string[] folders )
        {
            var result = folders.FirstOrDefault();
            foreach ( var folder in folders.Skip( 1 ) )
                result = System.IO.Path.Combine( result, folder );

            return result;
        }

    }
}
