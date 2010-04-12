using System;

namespace Laan.Persistence.Interfaces
{
    /// <summary>
    /// Provides a standard interface for DAOs which are data-access mechanism agnostic.
    /// 
    /// Since nearly all of the domain objects you create will have a type of int ID, this 
    /// base IDao leverages this assumption.  If you want a persistent object with a type 
    /// other than int, such as string, then use <see cref="IRepositoryWithTypedId{T, IdT}" />.
    /// </summary>
    public interface IRepository<T> : IRepositoryWithType<T, int> { }
}
