using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VLXD_API.Common;
using VLXD_API.DTOs.DangNhap;
using VLXD_API.DTOs.NguoiDung;
using VLXD_API.Models;
using VLXD_API.Services;

namespace VLXD_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XacThucNguoiDungController : ControllerBase
    {
        private readonly JwtService _jwtService;
        public XacThucNguoiDungController(JwtService jwtService) { 
            _jwtService = jwtService;
        }
        //[AllowAnonymous]
        //[HttpPost("login")]
        //public async Task<ActionResult<NguoiDungTokenDto>> Login(DangNhapDto request)
        //{
        //    var result = await _jwtService.Authenticate(request);
        //    if (result == null)
        //    {
        //        return Unauthorized();
        //    }
        //    return Ok(result);
        //}
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<NguoiDungTokenDto>>> Login(DangNhapDto request)
        {
            var result = await _jwtService.Authenticate(request);

            if (result == null)
            {
                // Trả về định dạng lỗi thống nhất của ApiResponse
                return Unauthorized(ApiResponse<NguoiDungTokenDto>.Fail("INVALID_CREDENTIALS", "Tên đăng nhập hoặc mật khẩu không chính xác"));
            }

            // Trả về định dạng thành công thống nhất: { success, data, meta, error }
            return Ok(ApiResponse<NguoiDungTokenDto>.Ok(result));
        }
    }
}
