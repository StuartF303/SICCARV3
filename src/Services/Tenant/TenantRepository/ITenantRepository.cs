using System.Linq.Expressions;
using Siccar.Platform.Tenants.Core;

namespace Siccar.Platform.Tenants.Repository
{
    /// <summary>
    /// Basic repos interface for generic data access.
    /// </summary>
    public interface ITenantRepository
    {
        Task<System.Linq.IQueryable<T>> All<T>() where T : class, new();
        Task<IQueryable<T>> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new();
        Task<T> Single<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task Delete<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task Add<T>(T item) where T : class, new();
        Task Add<T>(IEnumerable<T> items) where T : class, new();
        Task UpdateClient<T>(T item) where T : Client, new();
        Task UpdateTenant<T>(T item) where T : Tenant, new();
        bool CollectionExists<T>() where T : class, new();
        Task<bool> Exists<T>(Expression<Func<T, bool>> predicate);
    }
}