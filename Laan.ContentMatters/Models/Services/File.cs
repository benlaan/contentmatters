using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Principal;
using System.Runtime.InteropServices;

using Laan.ContentMatters.Models.Files;

namespace Laan.ContentMatters.Models.Services
{
    internal class WindowsAPI 
    {
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_LOGON_INTERACTIVE = 2;

        [DllImport( "advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
        public static extern bool LogonUser( String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken );

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
        public extern static bool CloseHandle( IntPtr handle );
    }

    public class PasswordNotSetException : Exception { }

    [Serializable]
    public class FileService : IDisposable
    {
        private IContext _httpContext;
        private WindowsIdentity _identity;
        private IntPtr _userToken;

        private const string Domain = "laanfamily";
        private const int PageSize = 100;

        public FileService( IContext httpContext )
        {
            _httpContext = httpContext;
            CalcIdentity();
        }        
        
        private void CalcIdentity()
        {
            string userName = _httpContext.Principal.Identity.Name;
            string password = (string) _httpContext.Session[ "password" ];

            if ( String.IsNullOrEmpty( password ) )
                throw new PasswordNotSetException();

            _userToken = IntPtr.Zero;

            if ( !WindowsAPI.LogonUser(
                userName,
                Domain,
                password,
                WindowsAPI.LOGON32_LOGON_INTERACTIVE, WindowsAPI.LOGON32_PROVIDER_DEFAULT,
                ref _userToken
            ) )
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new System.ComponentModel.Win32Exception( errorCode );
            }

            _identity = new WindowsIdentity( _userToken );
        }

        private StoredFileItem BuildStoredFileItem( string path )
        {
            StoredFileItem storedFileItem;
            int lineCount = 0;

            storedFileItem = new StoredFileItem();

            using ( var stream = new FileStream( path, FileMode.Open, FileAccess.Read ) )
            {
                using ( var sr = new StreamReader( stream ) )
                {
                    long position = 0;
                    int read = 0;
                    do
                    {
                        read = sr.BaseStream.ReadByte();

                        if ( read == -1 )
                            break;

                        if ( read == 13 )
                        {
                            lineCount++;

                            if ( lineCount % PageSize == 0 )
                            {
                                storedFileItem.Pages.Add( new FilePage() { Position = position } );
                                position = sr.BaseStream.Position;
                            }
                        }
                    }
                    while ( read != -1 );
                    storedFileItem.Pages.Add( new FilePage() { Position = position } );
                }
            }
            storedFileItem.PageCount = (int) Math.Ceiling( (double) lineCount / PageSize );
            return storedFileItem;
        }

        private void GetItems( string machineName, List<FileSystemItem> list, string path )
        {
            var directories = new List<string>();

            string searchPath = String.Format( "\\\\{0}{1}", machineName, path != "" ? "\\" + path : "" );
            var di = new DirectoryInfo( searchPath );
            directories.AddRange( di.GetDirectories().Select( d => d.FullName ) );

            list.AddRange(
                directories
                .Select(
                    dirName => new FileSystemItem()
                    {
                        Type = FileSystemType.Directory,
                        Name = dirName.Split( new[] { '\\', '/' } ).Last(),
                        Location = dirName.Replace( "\\\\", "" ).Replace( '\\', '/' )
                    }
                )
            );

            var excludedExtensions = new[] { "exe", "dll", "pdb", "sys", "com" };

            list.AddRange(
                di.GetFiles()
                .Where( file => !( excludedExtensions.Any( ext => ext == file.Extension.TrimStart( '.' ).ToLower() ) ) )
                .Select(
                    fileName => new FileSystemItem()
                    {
                        Type = FileSystemType.File,
                        Name = fileName.Name,
                        Location = fileName.FullName.Replace( "\\\\", "" ).Replace( '\\', '/' ),
                        Size = new FileInfo( fileName.FullName ).Length,
                        LastModified = fileName.LastWriteTime
                    }
                )
            );
        }

        public List<string> LoadFile( string path, int? page, out int totalPages )
        {
            using ( WindowsImpersonationContext wic = _identity.Impersonate() )
            {
                var lines = new List<string>();

                // if the session contains the paged positions, use it instead..
                StoredFileItem storedFileItem = (StoredFileItem) _httpContext.Session[ path ];

                if ( storedFileItem == null )
                {
                    storedFileItem = BuildStoredFileItem( path );
                    _httpContext.Session[ path ] = storedFileItem;
                }

                using ( FileStream stream = new FileStream( path, FileMode.Open, FileAccess.Read ) )
                {
                    stream.Seek( storedFileItem.Pages[ ( page ?? 1 ) - 1 ].Position, SeekOrigin.Begin );
                    using ( StreamReader sr = new StreamReader( stream ) )
                    {
                        while ( lines.Count < PageSize && !sr.EndOfStream )
                            lines.Add( sr.ReadLine().Replace( "\t", new String( ' ', 4 ) ) );
                    }
                }

                totalPages = storedFileItem.PageCount;
                return lines;
            }
        }

        internal List<FileSystemItem> LoadFilesAndDirectories( string machineName, string path )
        {
            using ( WindowsImpersonationContext wic = _identity.Impersonate() )
            {
                var result = new List<FileSystemItem>();
                GetItems( machineName, result, path );
                return result;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if ( _userToken != IntPtr.Zero )
                WindowsAPI.CloseHandle( _userToken );
        }

        #endregion
    }
}
