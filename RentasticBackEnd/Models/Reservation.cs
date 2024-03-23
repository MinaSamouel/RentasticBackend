namespace RentasticBackEnd.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserSsn { get; set; }
        public int CarId { get; set; }
        public DateTime StartRentTime { get; set; }
        public DateTime EndRentDate { get; set; }
        public double TotalPrice { get; set; }

        public Review Review { get; set; } = null!;
        public User User { get; set; } = null!;
        public Car Car { get; set; } = null!;
    }

}
