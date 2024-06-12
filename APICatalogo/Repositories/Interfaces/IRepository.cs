using System.Linq.Expressions;

namespace APICatalogo.Repositories.Interfaces;

// Repositório genérico
public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetAsync(Expression<Func<T, bool>> precidate);

    T Create(T entity);
    T Update(T entity);
    T Delete(T entity);
}
