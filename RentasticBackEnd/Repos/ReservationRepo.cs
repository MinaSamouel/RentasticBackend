namespace RentasticBackEnd.Repos
{
    public interface IReservationRepo
    {
        List<Reservation> GetAllReservation();
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

        public List<Reservation> GetAllReservation()
        {
            return _context.Reservations.ToList();
        }
        public Reservation GetReservationById(int id)
        {
            if (!ReservationExists(id))
            {
                return null;
            }
            return _context.Reservations.FirstOrDefault(u => u.Id == id);
        }

        public Reservation Add(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
            return reservation;
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
        public void Delete(int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                _context.SaveChanges();
            }
        }
    }
}
