namespace RentasticBackEnd.Models;

public class FavoriteCars
{
    public int Ssn { get; set; }

    public int CarId { get; set; }

    public User User { get; set; } = null!;
    public Car Car { get; set; } = null!;
}