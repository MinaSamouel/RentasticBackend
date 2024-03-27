using Microsoft.EntityFrameworkCore;
using RentasticBackEnd.Models;

namespace RentasticBackEnd.Repos;

public interface ICarRepo
{
    List<Car> GetAllCars();
    List<Car> GetCarsAdmin();
    List<Car> GerCarRserved(DateTime start); // Get cars that are res
    bool ExistsId(int id);
    Car? GetById(int id);
    Car? GetCarAdmin(int id);
    Car Add(Car car);
    Car Update(Car car);
    void Delete(Car car);
}

public class CarRepo : ICarRepo
{
    private readonly CarRentalContext _context;

    public CarRepo(CarRentalContext context)
    {
        _context = context;
    }

    public List<Car> GetAllCars()
    {
        return _context.Cars
            .Include(c => c.Reviews)
            .AsSplitQuery()
            .ToList();

        
    }

    public List<Car> GetCarsAdmin()
    {
        return _context.Cars
            .Include(c => c.Reviews)
            .AsSplitQuery()
            .Include(c => c.Reservations)
            .AsSplitQuery()
            .Include(c => c.FavoriteCars)
            .AsSplitQuery()
            .ToList();
    }

    public List<Car> GerCarRserved(DateTime start)
    {
        var cars = _context.Cars
            .Include(c => c.Reservations)
            .AsSplitQuery()
            .Where(c => c.Reservations.Any(r => r.StartRentTime <= start && r.EndRentDate >= start))
            .ToList();
        return cars;

    }

    public bool ExistsId(int id)
    {
        return _context.Cars.Any(e => e.Id == id);
    }

    public Car? GetById(int id)
    {
        if(!ExistsId(id))
            return null;

        return _context.Cars
            .Include(c => c.Reviews)
            .AsSplitQuery()
            .FirstOrDefault(c => c.Id == id);
    }

    public Car? GetCarAdmin(int id)
    {
        if (!ExistsId(id))
            return null;

        return _context.Cars
            .Include(c => c.Reviews)
            .AsSplitQuery()
            .Include(c => c.Reservations)
            .AsSplitQuery()
            .Include(c => c.FavoriteCars)
            .AsSplitQuery()
            .FirstOrDefault(c => c.Id == id);
    }

    public Car Add(Car car)
    {
        _context.Cars.Add(car);
        _context.SaveChanges();
        return car;
    }

    public Car Update(Car car)
    {
        _context.Cars.Update(car);
        _context.SaveChanges();
        return car;
    }

    public void Delete(Car car)
    {
        _context.Cars.Remove(car);
        _context.SaveChanges();
    }

}