using System.Collections.Immutable;

namespace RentasticBackEnd.IGenericRepo;

public interface IGenericRepository<T> where T : class
{
    ImmutableArray<T> GetAll();
    T? GetById(params object[] keyValues);
    T Add(T entity);
    T Update(T entity);
    bool Delete(T entity);
    bool Exists(params object[] keyValues);
}
