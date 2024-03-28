using Microsoft.EntityFrameworkCore;
using RentasticBackEnd.Models;

namespace RentasticBackEnd.Repos
{
    public interface IUserRepo
    {
        List<User> GetAllUsers();
        bool ExistsId(int id);
        bool ExistsEmail(string email);
        bool ExistsNationalNumber(string phoneNumber);
        User? GetOneById(int id);
        User? GetOneByEmail(string email);
        User? GetOneByNationalNumber(string nationalNumber);
        User Add(User user);
        User Update(User user);
        void Delete(User user);
        User? GetOneByIdUser(int id);
    }

    public class UserRepo : IUserRepo
    {
        private readonly CarRentalContext _context;

        public UserRepo(CarRentalContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.Reviews)
                .AsSplitQuery()
                .Include(u => u.Reservations)
                .AsSplitQuery()
                .Include(u => u.FavoriteCars)
                .AsSplitQuery()
                .ToList();
        }

        public bool ExistsId(int id)
        {
            return _context.Users.Any(e => e.Ssn == id);
        }

        public bool ExistsEmail(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }

        public bool ExistsNationalNumber(string nationalNumber)
        {
            return _context.Users.Any(e => e.NationalIdentityNumber == nationalNumber);
        }

        public User? GetOneById(int id)
        {
            if (!ExistsId(id))
                return null;
            return _context.Users.FirstOrDefault(u => u.Ssn == id);
        }

        public User? GetOneByIdUser(int id)
        {
            if (!ExistsId(id))
                return null;
            return _context.Users
                .Include(u => u.Reservations)
                .AsSplitQuery()
                .Include(u => u.Reviews)
                .AsSplitQuery()
                .Include(u => u.FavoriteCars)
                .AsSplitQuery()
                .FirstOrDefault(u => u.Ssn == id);
        }

        public User? GetOneByEmail(string email)
        {
            if (!ExistsEmail(email))
                return null;
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetOneByNationalNumber(string nationalNumber)
        {
            if (!ExistsNationalNumber(nationalNumber))
                return null;
            return _context.Users.FirstOrDefault(u => u.NationalIdentityNumber == nationalNumber);
        }

        public User Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

    }
}
