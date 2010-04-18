using System;

namespace Laan.ContentMatters.Engine
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException( string path ) : base( String.Format( "No Page found with name '{0}'", path ) ) { }
    }
}
