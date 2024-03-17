namespace RentasticBackEnd;

public class Review
{
    public int ReservationId { get; set; }
    public int CarId { get; set; }
    public int UserSsn { get; set; }

    public string Message { get; set; } = null!;
    public int Rate { get; set; }

    public Car Car { get; set; } = null!;
    public User User { get; set; } = null!;
    public Reservation Reservation { get; set; } = null!;
}