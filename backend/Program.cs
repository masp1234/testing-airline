using backend.Database;
using backend.Repositories;
using dotenv.net;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            DotEnv.Load();
            // Try to load a connection string from .env. If it does not exist, get it from an appsettings.json file.
            string? connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // Register / add repositories to the container
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Add services to the container.

            builder.Services.AddControllers();
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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
