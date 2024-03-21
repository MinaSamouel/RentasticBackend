using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentasticBackEnd;
using RentasticBackEnd.DTO;


public class CarRentalContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<FavoriteCars> FavoriteCars { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public CarRentalContext()
    {
        
    }

    public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options)
    {
        
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-QHPRGSQ;Initial Catalog=CarRental;Integrated Security=True;Trust Server Certificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Creating the Cars table
        // Creating the Cars table
        modelBuilder.Entity<Car>()
            .Property(c => c.Id)
            .HasColumnName("CarId");

        modelBuilder.Entity<Car>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Car>()
            .Property(c => c.Name)
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        modelBuilder.Entity<Car>()
            .Property(c => c.Brand)
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        modelBuilder.Entity<Car>()
            .Property(c => c.ModelYear)
            .HasColumnType("nvarchar(20)")
            .IsRequired();

        modelBuilder.Entity<Car>()
            .Property(c => c.Color)
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        modelBuilder.Entity<Car>()
            .Property(c => c.Category)
            .HasColumnType("nvarchar(20)")
            .IsRequired();

        modelBuilder.Entity<Car>()
            .Property(c => c.SeatCount)
            .HasColumnType("int")
            .IsRequired();

        modelBuilder.Entity<Car>()
            .Property(c => c.PricePerDay)
            .HasColumnType("int")
            .IsRequired();

        modelBuilder.Entity<Car>()
            .Property(c => c.Images)
            .IsRequired();

        modelBuilder.Entity<Car>()
            .Property(c => c.IsAutomatic)
            .HasDefaultValue(false)
            .IsRequired()
            .HasColumnType("bit");

        modelBuilder.Entity<Car>()
            .Property(c => c.HasAirCondition)
            .HasDefaultValue(true)
            .IsRequired()
            .HasColumnType("bit");

        #endregion

        #region Creating the Users table
        // Creating the Users table
        modelBuilder.Entity<User>()
            .Property(u => u.Ssn)
            .HasColumnName("Ssn");
        modelBuilder.Entity<User>()
            .HasKey(u => u.Ssn);

        modelBuilder.Entity<User>()
            .Property(u => u.Name)
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasColumnType("nvarchar(150)")
            .IsRequired()
            .HasConversion(
                v => v.ToLower(),
                v => v);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.PhoneNumber)
            .HasColumnType("nvarchar(20)")
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Address)
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Image)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.IsAdmin)
            .HasDefaultValue(false)
            .HasColumnType("bit");


        #endregion

        #region Creating the FavoriteCars table
        // Creating the FavoriteCars table
        modelBuilder.Entity<FavoriteCars>()
            .HasKey(fc => new { fc.Ssn, fc.CarId });

        modelBuilder.Entity<FavoriteCars>()
            .Property(fc => fc.Ssn)
            .HasColumnName("UserSsn");

        modelBuilder.Entity<FavoriteCars>()
            .HasOne(fc => fc.User)
            .WithMany(u => u.FavoriteCars)
            .HasForeignKey(fc => fc.Ssn);

        modelBuilder.Entity<FavoriteCars>()
            .HasOne(fc => fc.Car)
            .WithMany(c => c.FavoriteCars)
            .HasForeignKey(fc => fc.CarId);

        #endregion

        #region Creating the Reservation table
        //Creating the Reservation table
        modelBuilder.Entity<Reservation>()
            .Property(r => r.Id)
            .HasColumnName("ReservationId");

        modelBuilder.Entity<Reservation>()
            .Property(r => r.StartRentTime)
            .HasColumnType("date");

        modelBuilder.Entity<Reservation>()
            .Property(r => r.EndRentDate)
            .HasColumnType("date");

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserSsn);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Car)
            .WithMany(c => c.Reservations)
            .HasForeignKey(r => r.CarId);

        modelBuilder.Entity<Reservation>()
            .Property(r => r.TotalPrice)
            .HasColumnType("decimal(5,2)");
        #endregion

        #region Creating Review table
        //Creating Review table
        modelBuilder.Entity<Review>()
            .HasKey(rv => new { rv.ReservationId, rv.CarId, rv.UserSsn });

        modelBuilder.Entity<Review>()
            .Property(rv => rv.Message)
            .HasColumnName("Review_Message")
            .HasColumnType("nvarchar(1000)");

        modelBuilder.Entity<Review>()
            .Property(rv => rv.Rate)
            .HasColumnName("Review_Rate");

        modelBuilder.Entity<Review>()
            .HasOne(rv => rv.Reservation)
            .WithOne(re => re.Review)
            .HasForeignKey<Review>(rv => rv.ReservationId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Review>()
            .HasOne(rv => rv.Car)
            .WithMany(c => c.Reviews)
            .HasForeignKey(rv => rv.CarId);

        modelBuilder.Entity<Review>()
            .HasOne(rv => rv.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(rv => rv.UserSsn);
        #endregion

        base.OnModelCreating(modelBuilder);
    }

}