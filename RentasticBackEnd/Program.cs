using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentasticBackEnd.DTO;
using RentasticBackEnd.Repos;


namespace RentasticBackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            

            builder.Services.AddDbContext<CarRentalContext>(options =>
                               options.UseSqlServer(builder.Configuration.GetConnectionString("Database1")));


            //Add services for Repos
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IReservationRepo, ReservationRepo>();

            builder.Services.AddScoped<ICarRepo, CarRepo>();
            builder.Services.AddScoped<IFavouriteCarRepo, FavouriteCarRepo>();

            //Add services for FluentValidation for validate the models
            builder.Services.AddFluentValidationClientsideAdapters();
            builder.Services.AddScoped<IValidator<RegisterModel>, RegisterValidator>();
            builder.Services.AddScoped<IValidator<LoginModel>, LoginValidator>();
            builder.Services.AddScoped<IValidator<CarModel>, CarValidator>();
            builder.Services.AddScoped<IValidator<RentDateModel>, RentDateValidator>();
            builder.Services.AddScoped<IValidator<FavoriteCarsModel>, FavoriteCarsValidator>();


            //Add services for Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<CarRentalContext>();
                //.AddDefaultTokenProviders(); this line needed for change passord or Phone Number

            //[Authorize] used jwt Token in Authentication
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.RequireHttpsMetadata = false;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
