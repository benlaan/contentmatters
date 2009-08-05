using System.Security.Principal;

namespace Laan.ContentMatters.Models.Files
{
    public interface IContext
    {
        IPrincipal Principal { get; }
        ISession Session { get; set; }
    }
}
