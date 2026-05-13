using Mapster;
using VLXD_API.DTOs.ChiTietDonHang;
using VLXD_API.DTOs.ChiTietPhieuNhap;
using VLXD_API.DTOs.CongNoKhachHang;
using VLXD_API.DTOs.CongNoNcc;
using VLXD_API.DTOs.DanhMuc;
using VLXD_API.DTOs.DonHang;
using VLXD_API.DTOs.KhachHang;
using VLXD_API.DTOs.NguoiDung;
using VLXD_API.DTOs.NhaCungCap;
using VLXD_API.DTOs.PhieuNhapKho;
using VLXD_API.DTOs.SanPham;
using VLXD_API.Models;

namespace VLXD_API.Config;

public class EntityMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ChiTietDonHang, ChiTietDonHangDto>().TwoWays();

        config.NewConfig<ChiTietPhieuNhap, ChiTietPhieuNhapDto>().TwoWays();

        config.NewConfig<CongNoKhachHang, CongNoKhachHangDto>().TwoWays();

        config.NewConfig<CongNoNcc, CongNoNccDto>().TwoWays();

        config.NewConfig<DanhMuc, DanhMucDto>().TwoWays();

        config.NewConfig<DonHang, DonHangDto>().TwoWays();

        config.NewConfig<KhachHang, KhachHangDto>().TwoWays();

        config.NewConfig<NguoiDung, NguoiDungDto>().TwoWays();

        config.NewConfig<NhaCungCap, NhaCungCapDto>().TwoWays();

        config.NewConfig<PhieuNhapKho, PhieuNhapKhoDto>().TwoWays();

        config.NewConfig<SanPham, SanPhamDto>().TwoWays();
        config.NewConfig<SanPham, SanPhamTonKhoDto>().TwoWays();
    }
}
