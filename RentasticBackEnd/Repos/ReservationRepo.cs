using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace RentasticBackEnd.Repos
{
    public interface IReservationRepo
    {
        IEnumerable<Reservation> GetAllReservations();
        Reservation GetReservationById(int id);
        Reservation Add(Reservation reservation);
        void Delete(int id);
    }

    public class ReservationRepo : IReservationRepo
    {
        private readonly CarRentalContext _context;

        public ReservationRepo(CarRentalContext context)
        {
            _context = context;
        }

        public IEnumerable<Reservation> GetAllReservations()
        {
            return _context.Reservations.Include(r => r.User).Include(r => r.Car).ToList();
        }

        public Reservation GetReservationById(int id)
        {
            return _context.Reservations.Include(r => r.User).Include(r => r.Car).FirstOrDefault(r => r.Id == id);
        }

        public Reservation Add(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            return reservation;
        }

        public void Delete(int id)
        {
            var reservationToDelete = _context.Reservations.Find(id);
            if (reservationToDelete != null)
            {
                _context.Reservations.Remove(reservationToDelete);
                _context.SaveChanges();
            }
        }
    }
}
