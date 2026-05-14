using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VLXD_API.DTOs.DangNhap;
using VLXD_API.DTOs.NguoiDung;
using VLXD_API.Handlers;
using VLXD_API.Models;

namespace VLXD_API.Services
{
    public class JwtService
    {
        private readonly AppDbContext _Context;
        private readonly IConfiguration _configuration;

        public JwtService(AppDbContext Context, IConfiguration configuration)
        {
            _Context = Context;
            _configuration = configuration;
        }

        public async Task<NguoiDungTokenDto?> Authenticate(DangNhapDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Taikhoan) || string.IsNullOrWhiteSpace(request.Matkhau))
                return null;

            var userAccount = await _Context.NguoiDungs.FirstOrDefaultAsync(x => x.TenDangNhap == request.Taikhoan);

            if (userAccount is null || !BamMatKhauHandler.VerifyPassword(request.Matkhau, userAccount.MatKhauHash!))
                return null;

            string userRole;

            if (userAccount.VaiTro!=null)
            {
                userRole = userAccount.VaiTro;
            }
            else
            {
                userRole = "staff";
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, userAccount.MaNguoiDung.ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Name, request.Taikhoan),
                new Claim(ClaimTypes.Role,userRole)
            }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = _configuration["JwtConfig:Issuer"],
                Audience = _configuration["JwtConfig:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtConfig:Key"])), SecurityAlgorithms.HmacSha512Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new NguoiDungTokenDto
            {
                Token = accessToken
                //TenDangNhap = request.Taikhoan,
                //VaiTro = userRole   
            };
        }
    }
}
