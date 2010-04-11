using System;

using Laan.ContentMatters.Configuration;

namespace Laan.ContentMatters.Engine.Interfaces
{
    public interface IViewLoader
    {
        View Load( Page page );
    }
}
