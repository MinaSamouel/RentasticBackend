using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RentasticBackEnd.DTO;

namespace RentasticBackEnd.Repos;

public interface IFavouriteCarRepo
{
    bool FavouriteExists(int carId, int userId);
    void HandleFavourite(int carId, int userId);
}

public class FavouriteCarRepo : IFavouriteCarRepo
{
    private readonly CarRentalContext _context;

    public FavouriteCarRepo(CarRentalContext context)
    {
        _context = context;
    }

    public bool FavouriteExists(int carId, int userId)
    {
        return _context.FavoriteCars.Any(e => e.CarId == carId && e.Ssn == userId);
    }

    public void HandleFavourite(int carId, int userId)
    {
        if (FavouriteExists(carId, userId))
        {
            var favourite = _context.FavoriteCars.FirstOrDefault(e => e.CarId == carId && e.Ssn == userId);
            _context.FavoriteCars.Remove(favourite!);
        }
        else
        {
            _context.FavoriteCars.Add(new FavoriteCars { CarId = carId, Ssn = userId });
        }

        _context.SaveChanges();
    }

}