namespace RentasticBackEnd.Models;

public class Car
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? ModelYear { get; set; }
    public string? Color { get; set; }
    public string? Category { get; set; }
    public int SeatCount { get; set; }
    public int PricePerDay { get; set; }
    public string? Images { get; set; }
    public bool IsAutomatic { get; set; }
    public bool HasAirCondition { get; set; }
    public string? Description { get; set; }

    public ICollection<FavoriteCars> FavoriteCars { get; set; } = new HashSet<FavoriteCars>();
    public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();

    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();

}