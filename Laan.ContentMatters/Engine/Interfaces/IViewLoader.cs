using System;

using Laan.ContentMatters.Configuration;
using System.Collections.Generic;

namespace Laan.ContentMatters.Engine.Interfaces
{
    public interface IViewLoader
    {
        void GenerateData( Page page, IDictionary<string, object> contextData );
        View Load( Page page );
        IDataDictionary Data { get; set; }
    }
}
