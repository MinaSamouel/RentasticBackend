using Microsoft.EntityFrameworkCore;

namespace RentasticBackEnd.Repos
{
    public interface IUserRepo
    {
        List<User> GetAllUsers();
        bool ExistsId(int id);
        bool ExistsEmail(string email);
        bool ExistsPhoneNumber(string phoneNumber);
        User? GetOneById(int id);
        User? GetOneByEmail(string email);
        User? GetOneByPhoneNumber(string phoneNumber);
        User Add(User user);
        User Update(User user);
        void Delete(User user);
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

        public bool ExistsPhoneNumber(string phoneNumber)
        {
            return _context.Users.Any(e => e.PhoneNumber == phoneNumber);
        }

        public User? GetOneById(int id)
        {
            if (!ExistsId(id))
                return null;
            return _context.Users.FirstOrDefault(u => u.Ssn == id);
        }

        public User? GetOneByEmail(string email)
        {
            if (!ExistsEmail(email))
                return null;
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetOneByPhoneNumber(string phoneNumber)
        {
            if (!ExistsPhoneNumber(phoneNumber))
                return null;
            return _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
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
