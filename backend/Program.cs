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
using System.Text;
using Sentry.Extensibility;
using backend.Database.Data.MongoDB;
using backend.Repositories.MongoDB;
using MongoDB.Driver;
using backend.Repositories.Neo4j;
using Neo4jClient;

namespace backend
{
    public class Program
	{
		public static void Main(string[] args)
		{
			DotEnv.Load();

			var builder = WebApplication.CreateBuilder(args);
	  
	  // Initializing and configuring Sentry
	  builder.WebHost.UseSentry(options =>
			{
				options.TracesSampleRate = 0.5;
				options.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN") ?? "";
				options.MaxRequestBodySize = RequestSize.Medium;
				options.MinimumBreadcrumbLevel = LogLevel.Debug;
				options.AttachStacktrace = true;
				options.Debug = true;
				options.DiagnosticLevel = SentryLevel.Error;
				options.CaptureFailedRequests = true;
				options.SendDefaultPii = false;

				options.SetBeforeSend((sentryEvent, hint) =>
				{
					if (sentryEvent != null)
					{
						sentryEvent.ServerName = null;
						sentryEvent.User.IpAddress = null;
					}
					return sentryEvent;
				});
			});

			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(
					policy =>
					{
						policy.WithOrigins(Environment.GetEnvironmentVariable("CLIENT_URL") ?? "http://localhost:5173")
							  .AllowCredentials()
							  .AllowAnyHeader()
							  .AllowAnyMethod();
					});
			});
			// Try to load a connection string from .env. If it does not exist, get it from an appsettings.json file.
			string? connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("Default");
			builder.Services.AddDbContext<DatabaseContext>(options =>
			{
				options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(connectionString));
				options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(connectionString));
			});


			string? mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING")?? "";

			builder.Services.AddDbContext<MongoDBContext>(options =>
			{
				options.UseMongoDB(mongoDbConnectionString, "mydatabase");
			});




			builder.Services.AddSingleton<IGraphClient>(provider =>
			{
				var neo4jUrl = Environment.GetEnvironmentVariable("NEO4J_URL")?? "";
				var neo4jUsername = Environment.GetEnvironmentVariable("NEO4J_USERNAME");
				var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
				var client = new BoltGraphClient(new Uri(neo4jUrl), neo4jUsername, neo4jPassword);
				client.ConnectAsync().Wait(); // Ensure the client connects to the database
				return client;
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
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTSecretKey") ?? ""))
				};

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						// Checks for the JWT in the cookie
						var token = context.HttpContext.Request.Cookies["AuthToken"];
						if (!string.IsNullOrEmpty(token))
						{
							context.Token = token;
						}
						return Task.CompletedTask;
					}
				};
			});
			
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("RequireAuthenticatedUser", policy =>
					policy.RequireAuthenticatedUser());
			});
			
			builder.Services.AddControllersWithViews();
	  
	  // Add HTTP client for Google Distance API
	  builder.Services.AddHttpClient<IDistanceApiService, DistanceApiService>();

			

			// Register repository dependency injection depending on database type
			string? databaseType = Environment.GetEnvironmentVariable("DB_TYPE");

			switch (databaseType)
			{
				case "MySQL":
					builder.Services.AddScoped<IAirplaneRepository, AirplaneRepository>();        
                    builder.Services.AddScoped<IFlightRepository, FlightRepository>();
                    builder.Services.AddScoped<IBookingRepository, BookingRepository>();
                    builder.Services.AddScoped<IUserRepository, UserRepository>();
                    builder.Services.AddScoped<IAirportRepository, AirportRepository>();
                    builder.Services.AddScoped<IAirlineRepository, AirlineRepository>();
                    break;
				case "MongoDB":
                    builder.Services.AddScoped<IAirplaneRepository, AirplaneMongoDBRepository>();
					builder.Services.AddScoped<IFlightRepository, FlightsMongoDBRepository>();
					builder.Services.AddScoped<IBookingRepository, BookingMongoDBRepository>();
                    builder.Services.AddScoped<IUserRepository, UserMongoDBRepository>();
                    builder.Services.AddScoped<IAirportRepository, AirportMongoDBRepository>();
                    builder.Services.AddScoped<IAirlineRepository, AirlineMongoDBRepository>();

                    // Register MongoDB seeder
                    builder.Services.AddTransient<MongoDBSeeder>();

                    // Register IMongoclient for the MongoDB seeder
                    builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
                    {
                        return new MongoClient(mongoDbConnectionString);
                    });

                    break;
                case "Neo4j":

					builder.Services.AddScoped<IAirlineRepository, AirlineNeo4jRepository>();
					builder.Services.AddScoped<IAirplaneRepository, AirplaneNeo4jRepository>();
					builder.Services.AddScoped<IAirportRepository, AirportNeo4jRepository>();
					builder.Services.AddScoped<IUserRepository, UserNeo4jRepository>();
					builder.Services.AddScoped<IFlightRepository, FlightNeo4jRepository>();
					builder.Services.AddScoped<IBookingRepository, BookingNeo4jRepository>();
                    
               
					break;
                default:
					throw new ArgumentException("Need to specify a database type by prodiving the DB_TYPE environment variable ('MySQL' | 'MongoDB' | 'Neo4j').");
            }

			// Register repositories for the DI container

            
            
            

            // Add services to the container.
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFlightService, FlightService>();
            builder.Services.AddScoped<IAirportService, AirportService>();
            builder.Services.AddScoped<IAirplaneService, AirplaneService>();
            builder.Services.AddScoped<IAirlineService, AirlineService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
			builder.Services.AddScoped<ITicketAvailabilityChecker, TicketAvailabilityChecker>();
			builder.Services.AddScoped<IEmailService, EmailService>();
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
				Secure = CookieSecurePolicy.Always // Ensure this is set to None when using localhost
			});
			
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();

			if (databaseType == "MongoDB")
			{
				// Seed the MongoDB database
				using (var scope = app.Services.CreateScope())
                {
                    var mongoSeeder = scope.ServiceProvider.GetRequiredService<MongoDBSeeder>();
                    mongoSeeder.Seed();
                }

            }
            

            app.Run();
		}
	}
}