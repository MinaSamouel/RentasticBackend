﻿using System.Collections.ObjectModel;

namespace RentasticBackEnd;

public class User
{
    public int Ssn { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Image { get; set; } = null!;
    public bool IsAdmin { get; set; }

    public ICollection<FavoriteCars> FavoriteCars { get; set; } = new HashSet<FavoriteCars>();
    //public List<Car> Cars { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();

    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}