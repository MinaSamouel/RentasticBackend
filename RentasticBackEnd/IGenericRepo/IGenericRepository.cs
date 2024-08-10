using System.Collections.Immutable;
using System.Linq.Expressions;

namespace RentasticBackEnd.IGenericRepo;

public interface IGenericRepository<T> where T : class
{
    ImmutableArray<T> GetAll();
    T? GetById(Expression<Func<T, bool>>? filter, Expression<Func<T, object>>[]? include);
    T Add(T entity);
    T Update(T entity);
    bool Delete(T entity);
    bool Exists(params object[] keyValues);
}
