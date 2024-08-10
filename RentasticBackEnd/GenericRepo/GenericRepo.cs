using Microsoft.EntityFrameworkCore;
using RentasticBackEnd.IGenericRepo;
using RentasticBackEnd.Models;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace RentasticBackEnd.GenericRepo;

public class GenericRepository<T> : IGenericRepository<T> where T : class 
{
    private readonly CarRentalContext _context;
    private readonly DbSet<T> _dbset;

    public GenericRepository(CarRentalContext context)
    {
        _context = context;
        _dbset = context.Set<T>();
    }
    public ImmutableArray<T> GetAll()
    {
        return _dbset.ToImmutableArray();
    }

    public T? GetById(Expression<Func<T, bool>>? filter,
        Expression<Func<T, object>>[]? include)
    {
        IQueryable<T> query = _dbset;

        if (include != null && include.Length != 0)
        {
            foreach (var item in include)
            {
                query = query.Include(item);
            }
        }

        if (filter != null)
        {
            return query.FirstOrDefault(filter);
        }
        
        return query.FirstOrDefault();
    }

    public T Add(T entity)
    {
        _dbset.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public T Update(T entity)
    {
        _dbset.Update(entity);
        _context.SaveChanges();
        return entity;
    }

    public bool Delete(T entity)
    {
        _dbset.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Exists(params object[] keyValues)
    {
        return _dbset.Find(keyValues) != null;
    }
}
