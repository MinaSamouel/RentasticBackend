namespace RentasticBackEnd
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserSsn { get; set; }
        public int CarId { get; set; }
        public DateTime StartRentTime { get; set; }
        public DateTime EndRentDate { get; set; }
        public double TotalPrice { get; set; }

        public Review Review { get; set; }
        public User User { get; set; }
        public Car Car { get; set; }
    }

}
