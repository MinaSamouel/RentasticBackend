namespace RentasticBackEnd.Repos
{
    public interface IReviewRepo
    {
        List<Review> GetAllReviews();

        // Review GetReviewById(int id);

        Review? GetReviewById(int carId,int userSsn, int reservationId);
        Review Add(Review review);
        Review Update(Review review);
    }
    public class ReviewRepo : IReviewRepo
    {
        private readonly CarRentalContext _context;

        public ReviewRepo(CarRentalContext context)
        {
            _context = context;
        }

        public List<Review> GetAllReviews()
        {
            return _context.Reviews.ToList();
        }
        //public bool ExistsId(int id)
        //{
        //    return _context.Reviews.Any(e => e.UserSsn == id);
        //}
        //public Review? GetReviewById(int id)
        //{
        //    if (!ExistsId(id))
        //        return null;
        //    return _context.Reviews.FirstOrDefault(e=>e.UserSsn == id);
        //}
        public Review? GetReviewById(int carId, int userSsn, int reservationId)
        {
            return _context.Reviews.FirstOrDefault(r =>
                r.CarId == carId
                && r.UserSsn == userSsn
                && r.ReservationId == reservationId);
        }



        public Review Add(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return review;
        }

        public Review Update(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
            return review;
        }

    }
}
