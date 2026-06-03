using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VLXD_API.Models;
using VLXD_API.Services;

namespace VLXD_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
                        Environment.SetEnvironmentVariable(
                "DOTNET_USE_POLLING_FILE_WATCHER",
                "1"
            );
            // Tắt reloadOnChange để tránh FileSystemWatcher trên Render
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args
            });

            builder.Configuration.Sources.Clear();

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile(
                    $"appsettings.{builder.Environment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: false)
                .AddEnvironmentVariables();

            // Render/Railway Port
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            // Controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Mapster
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            mapsterConfig.Scan(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddSingleton(mapsterConfig);
            builder.Services.AddScoped<IMapper, ServiceMapper>();

            // Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("ConnectionStrings:DefaultConnection chưa được cấu hình.");
            }

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString));
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // JWT
            var jwtKey = builder.Configuration["JwtConfig:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception("JwtConfig:Key chưa được cấu hình.");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                        ValidAudience = builder.Configuration["JwtConfig:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<JwtService>();

            var app = builder.Build();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VLXD API v1");
                c.RoutePrefix = "swagger";
            });

            // CORS
            app.UseCors("AllowAll");

            // Auth
            app.UseAuthentication();
            app.UseAuthorization();

            // Controllers
            app.MapControllers();

            // Health Check
            app.MapGet("/", () => "VLXD API Running");
            app.MapGet("/health", () => Results.Ok("OK"));
            app.MapGet("/heath", () => Results.Ok("OK"));

            app.Run();
        }
    }
}