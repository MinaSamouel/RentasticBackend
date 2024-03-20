namespace RentasticBackEnd.DTO
{
    public class ReviewModel
    {
        public int ReservationId { get; set; }
        public int CarId { get; set; }
        public int UserSsn { get; set; }

        public string Message { get; set; } = null!;
        public int Rate { get; set; }
    }
}
