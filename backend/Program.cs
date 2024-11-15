using System.Text;
using backend.Config;
using backend.Database;
using backend.Models;
using backend.Repositories;
using backend.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173")
                              .AllowCredentials()
                              .AllowAnyHeader();
                    });
            });
            // Try to load a connection string from .env. If it does not exist, get it from an appsettings.json file.
            string? connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            
            ///////
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("Issuer"),
                    ValidAudience = Environment.GetEnvironmentVariable("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTSecretKey")))
                };
            });
            builder.Services.AddControllersWithViews();

            

            // Register / add repositories to the container
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IFlightRepository, FlightRepository>();
            builder.Services.AddScoped<IAirportRepository, AirportRepository>();
            builder.Services.AddScoped<IAirplaneRepository, AirplaneRepository>();

            // Add services to the container.
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFlightService, FlightService>();
            builder.Services.AddScoped<IAirportService, AirportService>();
            builder.Services.AddScoped<IAirplaneService, AirplaneService>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            app.UseHttpsRedirection();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always
            });
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllers();

            app.Run();
        }
    }
}
