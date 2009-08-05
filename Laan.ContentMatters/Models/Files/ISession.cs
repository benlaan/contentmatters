using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Principal;
using System.Reflection;

namespace Laan.ContentMatters.Models.Files
{
    public interface ISession
    {
        object this[ int index ] { get; set; }
        object this[ string name ] { get; set; }
    }
}
