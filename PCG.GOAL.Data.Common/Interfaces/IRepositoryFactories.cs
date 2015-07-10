using System;
using System.Data.Entity;
using DataAccess.Common.Helpers;

namespace DataAccess.Common.Interfaces
{
    public interface IRepositoryFactories
    {
        /// <summary>
        /// Get the repository factory function for the type.
        /// </summary>
        /// <typeparam name="T">Type serving as the repository factory lookup key.</typeparam>
        /// <returns>The repository function if found, else null.</returns>
        /// <remarks>
        /// The type parameter, T, is typically the repository type 
        /// but could be any type (e.g., an entity type)
        /// </remarks>
        Func<DbContext, object> GetRepositoryFactory<T>();

        /// <summary>
        /// Get the factory for <see cref="IRepository{T}"/> where T is an entity type.
        /// </summary>
        /// <typeparam name="T">The root type of the repository, typically an entity type.</typeparam>
        /// <returns>
        /// A factory that creates the <see cref="IRepository{T}"/>, given an EF <see cref="DbContext"/>.
        /// </returns>
        /// <remarks>
        /// Looks first for a custom factory in <see cref="RepositoryFactories._repositoryFactories"/>.
        /// If not, falls back to the <see cref="RepositoryFactories.DefaultEntityRepositoryFactory{T}"/>.
        /// You can substitute an alternative factory for the default one by adding
        /// a repository factory for type "T" to <see cref="RepositoryFactories._repositoryFactories"/>.
        /// </remarks>
        Func<DbContext, object> GetRepositoryFactoryForEntityType<T>() where T : class;
    }
}
