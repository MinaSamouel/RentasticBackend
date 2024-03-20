namespace RentasticBackEnd.DTO
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public int UserSsn { get; set; }
        public int CarId { get; set; }
        public DateTime StartRentTime { get; set; }
        public DateTime EndRentDate { get; set; }
        public double TotalPrice { get; set; }
    }
}
