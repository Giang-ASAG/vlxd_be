using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VLXD_API.Config;
using VLXD_API.Models;
using VLXD_API.Services;

namespace VLXD_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình cổng PORT động phục vụ cho việc Deploy Cloud (Render/Railway)
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Cấu hình Mapster
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            mapsterConfig.Scan(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddSingleton(mapsterConfig);
            builder.Services.AddScoped<IMapper, ServiceMapper>();

            // Cấu hình Database PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");

                options.UseNpgsql(connectionString);
            });

            // FIX LỖI CORS: Đổi tên biến builder bên trong thành policy để tránh trùng lặp
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Cấu hình JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                    ValidAudience = builder.Configuration["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<JwtService>();

            var app = builder.Build();

            // Kích hoạt CORS ngay đầu pipeline để đảm bảo mọi request đều qua bộ lọc này trước
            app.UseCors("AllowAll");

            // Mở Swagger cho cả môi trường Dev lẫn Production để tiện test API
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VLXD API v1");
                c.RoutePrefix = "swagger"; // Truy cập bằng đường dẫn domain.com/swagger
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Hỗ trợ cả 2 endpoint để tương thích hoàn toàn với script ping chống ngủ đông của bạn
            app.MapGet("/health", () => Results.Ok("OK"));
            app.MapGet("/heath", () => Results.Ok("OK"));

            app.Run();
        }
    }
}