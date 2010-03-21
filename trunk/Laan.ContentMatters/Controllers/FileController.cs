using System;
using System.Linq;
using System.Web.Mvc;

using Laan.ContentMatters.Models.Services;
using FileView = Laan.ContentMatters.Models.Files;
using Laan.ContentMatters.Models.Files;

namespace Laan.ContentMatters.Controllers
{
    public class FileController : BaseController, IDisposable
    {
        private FileService _fileService;

        internal FileService Service
        {
            get
            {
                if ( _fileService == null )
                    _fileService = new FileService();

                return _fileService;
            }
        }

        public ActionResult List( string machineName, string path )
        {
            if ( machineName == "" )
                machineName = Environment.MachineName;

            if ( path == "" )
                path = "c$";

            //try
            {
                ViewData.Model = new PathItem()
                {
                    Location = System.IO.Path.Combine( machineName, path ),
                    Files = Service.LoadFilesAndDirectories( machineName, String.IsNullOrEmpty( path ) ? "" : path )
                };

                return View();
            }
            //catch ( PasswordNotSetException )
            //{
            //    return RedirectToAction( "LogOn", "Account" );
            //}
        }

        public ActionResult View( string machineName, string path, int? page )
        {
            var totalPages = 0;
            var fullPath = String.Format( @"\\{0}\{1}", machineName, path.TrimEnd( '/' ) );

            var contents = Service
                .LoadFile( fullPath, page ?? 1, out totalPages  )
                .Select( con => Server.HtmlEncode( con ) )
                .ToList();

            //try
            {
                ViewData.Model = new FileItem()
                {
                    Contents = contents,
                    CurrentPage = page ?? 1,
                    TotalPages = totalPages,
                    FileSystemItem = new FileSystemItem()
                    {
                        Location = fullPath.Replace( "\\\\", "" ).Replace( '\\', '/' ),
                        Name = path.Split( new[] { '\\', '/' } ).Last(),
                    }
                };

                return View();
            }
            //catch ( PasswordNotSetException )
            //{
            //    return RedirectToAction( "LogOn", "Account" );
            //}
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if ( _fileService != null )
                _fileService.Dispose();
        }
       #endregion
    }
}
