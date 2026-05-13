
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

            // Add services to the container.
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            mapsterConfig.Scan(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddSingleton(mapsterConfig);
            builder.Services.AddScoped<IMapper, ServiceMapper>();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("DefaultConnection is missing in appsettings.json.");

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // Cấu hình CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            // Cấu hình JWT
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

            app.UseCors("AllowAll");


                app.UseSwagger();
                app.UseSwaggerUI();

            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseHttpsRedirection();
            //}



            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
