using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.IO;
using System.Text;

namespace Laan.ContentMatters.Models.Files
{
    public enum FileSystemType
    {
        File,
        Directory
    }
    
    public class FileSystemItem : BaseItem
    {
        public FileSystemType Type { get; set; }
        public string Name { get; set; }

        public string GetLink()
        {
            return String.Format(
                "/File/{0}/{1}",
                ( Type == FileSystemType.Directory ? "List" : "View" ),
                Location
            );
        }

        public DateTime? LastModified { get; set; }
        public long Size { get; set; }

        public string DisplaySize
        {
            get {

                if ( Type == FileSystemType.Directory)
                    return "";

                if ( Size > 1000000 )
                    return String.Format( "{0:0.00} MB", Size / 1000000.0 );

                if ( Size > 1000 )
                    return String.Format( "{0:0.00} KB", Size / 1000.0 );

                return String.Format( "{0} bytes", Size );
                
            }
        }

        public override string ToString()
        {
            return String.Format( "{0} / {1} ({2})", Name, Location, Type );
        }
    }

    public class PathItem : BaseItem
    {
        /// <summary>
        /// Initializes a new instance of the PathItem class.
        /// </summary>
        public PathItem()
        {
            Files = new List<FileSystemItem>();
        }

        public List<FileSystemItem> Files { get; set; }
    }

    public class FileItem
    {
        public FileSystemItem FileSystemItem { get; set; }
        public List<string> Contents { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class FilePage
    {
        public string Name { get; set; }
        public long Position { get; set; }
    }

    public class StoredFileItem
    {
        public StoredFileItem()
        {
            Pages = new List<FilePage>();
        }

        public int PageCount { get; set; }
        public List<FilePage> Pages { get; set; }
    }
}
