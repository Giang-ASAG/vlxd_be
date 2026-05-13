using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VLXD_API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "danh_muc",
                columns: table => new
                {
                    ma_danh_muc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ten_danh_muc = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mo_ta = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_danh_muc);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "khach_hang",
                columns: table => new
                {
                    ma_khach_hang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ho_ten = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    so_dien_thoai = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dia_chi = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    han_muc_no = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, defaultValueSql: "'0.00'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_khach_hang);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "kho",
                columns: table => new
                {
                    ma_kho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ten_kho = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dia_chi = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    so_dien_thoai_kho = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    trang_thai = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_kho);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "lichsuthanhtoan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IsNhaCungCap = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    conNoID = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PhuongThucThanhToan = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false, comment: "0: tien_mat, 1: chuyen_khoan"),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    GhiChu = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "nguoi_dung",
                columns: table => new
                {
                    ma_nguoi_dung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ten_dang_nhap = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mat_khau_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ho_ten = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vai_tro = table.Column<string>(type: "enum('admin','ban_hang','thu_kho','ke_toán')", nullable: true, defaultValueSql: "'ban_hang'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_nguoi_dung);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "nha_cung_cap",
                columns: table => new
                {
                    ma_ncc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ten_ncc = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    so_dien_thoai = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dia_chi = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ghi_chu = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_ncc);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "cong_no_khach_hang",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_khach_hang = table.Column<int>(type: "int", nullable: true),
                    ma_don_hang = table.Column<int>(type: "int", nullable: true),
                    so_tien_no = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ngay_phat_sinh = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    trang_thai = table.Column<string>(type: "enum('dang_no','hoan_tat')", nullable: true, defaultValueSql: "'dang_no'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_cnkh_kh",
                        column: x => x.ma_khach_hang,
                        principalTable: "khach_hang",
                        principalColumn: "ma_khach_hang");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "don_hang",
                columns: table => new
                {
                    ma_don_hang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_khach_hang = table.Column<int>(type: "int", nullable: true),
                    ma_nguoi_tao = table.Column<int>(type: "int", nullable: true),
                    ngay_tao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    tong_tien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    trang_thai_thanh_toan = table.Column<string>(type: "enum('chua_thanh_toan','thanh_toan_mot_phan','da_thanh_toan')", nullable: true, defaultValueSql: "'chua_thanh_toan'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    so_tien_tra = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    hinh_thuc = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_don_hang);
                    table.ForeignKey(
                        name: "fk_dh_kh",
                        column: x => x.ma_khach_hang,
                        principalTable: "khach_hang",
                        principalColumn: "ma_khach_hang");
                    table.ForeignKey(
                        name: "fk_dh_nv",
                        column: x => x.ma_nguoi_tao,
                        principalTable: "nguoi_dung",
                        principalColumn: "ma_nguoi_dung");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "cong_no_ncc",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_ncc = table.Column<int>(type: "int", nullable: true),
                    ma_phieu_nhap = table.Column<int>(type: "int", nullable: true),
                    so_tien_no = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ngay_phat_sinh = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    trang_thai = table.Column<string>(type: "enum('dang_no','hoan_tat')", nullable: true, defaultValueSql: "'dang_no'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_cnncc_ncc",
                        column: x => x.ma_ncc,
                        principalTable: "nha_cung_cap",
                        principalColumn: "ma_ncc");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "phieu_nhap_kho",
                columns: table => new
                {
                    ma_phieu_nhap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_ncc = table.Column<int>(type: "int", nullable: true),
                    ma_kho_nhap = table.Column<int>(type: "int", nullable: true),
                    ma_nguoi_lap = table.Column<int>(type: "int", nullable: true),
                    ngay_nhap = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    tong_tien_nhap = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    da_thanh_toan_ncc = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    trang_thai = table.Column<string>(type: "enum('cho_nhap','da_nhap_kho','da_huy')", nullable: true, defaultValueSql: "'cho_nhap'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_phieu_nhap);
                    table.ForeignKey(
                        name: "fk_pnk_kho",
                        column: x => x.ma_kho_nhap,
                        principalTable: "kho",
                        principalColumn: "ma_kho");
                    table.ForeignKey(
                        name: "fk_pnk_ncc",
                        column: x => x.ma_ncc,
                        principalTable: "nha_cung_cap",
                        principalColumn: "ma_ncc");
                    table.ForeignKey(
                        name: "fk_pnk_nv",
                        column: x => x.ma_nguoi_lap,
                        principalTable: "nguoi_dung",
                        principalColumn: "ma_nguoi_dung");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "san_pham",
                columns: table => new
                {
                    ma_san_pham = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_sku = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ten_san_pham = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ma_danh_muc = table.Column<int>(type: "int", nullable: true),
                    ma_ncc_mac_dinh = table.Column<int>(type: "int", nullable: true),
                    don_vi_chinh = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    gia_nhap_gan_nhat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    gia_ban_le = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    thue = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    gia_sau_thue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    so_luong = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ton_kho_toi_thieu = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'10'"),
                    ton_kho_toi_da = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'500'"),
                    ngay_tao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_san_pham);
                    table.ForeignKey(
                        name: "fk_sp_danhmuc",
                        column: x => x.ma_danh_muc,
                        principalTable: "danh_muc",
                        principalColumn: "ma_danh_muc");
                    table.ForeignKey(
                        name: "fk_sp_ncc",
                        column: x => x.ma_ncc_mac_dinh,
                        principalTable: "nha_cung_cap",
                        principalColumn: "ma_ncc");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "phieu_xuat_kho",
                columns: table => new
                {
                    ma_phieu_xuat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_don_hang = table.Column<int>(type: "int", nullable: true),
                    ma_kho_xuat = table.Column<int>(type: "int", nullable: true),
                    ma_nguoi_xuat = table.Column<int>(type: "int", nullable: true),
                    ngay_xuat = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    trang_thai_xuat = table.Column<string>(type: "enum('cho_xuat','da_xuat_kho')", nullable: true, defaultValueSql: "'cho_xuat'", collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ma_phieu_xuat);
                    table.ForeignKey(
                        name: "fk_pxk_dh",
                        column: x => x.ma_don_hang,
                        principalTable: "don_hang",
                        principalColumn: "ma_don_hang");
                    table.ForeignKey(
                        name: "fk_pxk_kho",
                        column: x => x.ma_kho_xuat,
                        principalTable: "kho",
                        principalColumn: "ma_kho");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "chi_tiet_don_hang",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_don_hang = table.Column<int>(type: "int", nullable: true),
                    ma_san_pham = table.Column<int>(type: "int", nullable: true),
                    so_luong = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    don_vi_tinh = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    don_gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    thanh_tien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, computedColumnSql: "`so_luong` * `don_gia`", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_ctdh_dh",
                        column: x => x.ma_don_hang,
                        principalTable: "don_hang",
                        principalColumn: "ma_don_hang");
                    table.ForeignKey(
                        name: "fk_ctdh_sp",
                        column: x => x.ma_san_pham,
                        principalTable: "san_pham",
                        principalColumn: "ma_san_pham");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "chi_tiet_phieu_nhap",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_phieu_nhap = table.Column<int>(type: "int", nullable: true),
                    ma_san_pham = table.Column<int>(type: "int", nullable: true),
                    so_luong = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    gia_nhap = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    loai_nhap = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValue: false),
                    thanh_tien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, computedColumnSql: "`so_luong` * `gia_nhap`", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_ctpn_pn",
                        column: x => x.ma_phieu_nhap,
                        principalTable: "phieu_nhap_kho",
                        principalColumn: "ma_phieu_nhap");
                    table.ForeignKey(
                        name: "fk_ctpn_sp",
                        column: x => x.ma_san_pham,
                        principalTable: "san_pham",
                        principalColumn: "ma_san_pham");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "don_vi_quy_doi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_san_pham = table.Column<int>(type: "int", nullable: true),
                    ten_don_vi = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ty_le_quy_doi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    gia_ban_theo_dv = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_quydoi_sp",
                        column: x => x.ma_san_pham,
                        principalTable: "san_pham",
                        principalColumn: "ma_san_pham");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "the_kho",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ma_san_pham = table.Column<int>(type: "int", nullable: true),
                    ma_kho = table.Column<int>(type: "int", nullable: true),
                    ngay_thay_doi = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    loai_giao_dich = table.Column<string>(type: "enum('NHAP_HANG','XUAT_HANG','KIEM_KE','TRA_HANG')", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    so_luong_thay_doi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    so_luong_ton_sau_khi_thay_doi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ma_chung_tu_lien_quan = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_tk_kho",
                        column: x => x.ma_kho,
                        principalTable: "kho",
                        principalColumn: "ma_kho");
                    table.ForeignKey(
                        name: "fk_tk_sp",
                        column: x => x.ma_san_pham,
                        principalTable: "san_pham",
                        principalColumn: "ma_san_pham");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ton_kho_chi_tiet",
                columns: table => new
                {
                    ma_kho = table.Column<int>(type: "int", nullable: false),
                    ma_san_pham = table.Column<int>(type: "int", nullable: false),
                    so_luong_ton = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    vi_tri_cu_the = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ma_kho, x.ma_san_pham })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_ton_kho",
                        column: x => x.ma_kho,
                        principalTable: "kho",
                        principalColumn: "ma_kho");
                    table.ForeignKey(
                        name: "fk_ton_sp",
                        column: x => x.ma_san_pham,
                        principalTable: "san_pham",
                        principalColumn: "ma_san_pham");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "fk_ctdh_dh",
                table: "chi_tiet_don_hang",
                column: "ma_don_hang");

            migrationBuilder.CreateIndex(
                name: "fk_ctdh_sp",
                table: "chi_tiet_don_hang",
                column: "ma_san_pham");

            migrationBuilder.CreateIndex(
                name: "fk_ctpn_pn",
                table: "chi_tiet_phieu_nhap",
                column: "ma_phieu_nhap");

            migrationBuilder.CreateIndex(
                name: "fk_ctpn_sp",
                table: "chi_tiet_phieu_nhap",
                column: "ma_san_pham");

            migrationBuilder.CreateIndex(
                name: "fk_cnkh_kh",
                table: "cong_no_khach_hang",
                column: "ma_khach_hang");

            migrationBuilder.CreateIndex(
                name: "fk_cnncc_ncc",
                table: "cong_no_ncc",
                column: "ma_ncc");

            migrationBuilder.CreateIndex(
                name: "fk_dh_kh",
                table: "don_hang",
                column: "ma_khach_hang");

            migrationBuilder.CreateIndex(
                name: "fk_dh_nv",
                table: "don_hang",
                column: "ma_nguoi_tao");

            migrationBuilder.CreateIndex(
                name: "fk_quydoi_sp",
                table: "don_vi_quy_doi",
                column: "ma_san_pham");

            migrationBuilder.CreateIndex(
                name: "so_dien_thoai",
                table: "khach_hang",
                column: "so_dien_thoai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ten_dang_nhap",
                table: "nguoi_dung",
                column: "ten_dang_nhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_pnk_kho",
                table: "phieu_nhap_kho",
                column: "ma_kho_nhap");

            migrationBuilder.CreateIndex(
                name: "fk_pnk_ncc",
                table: "phieu_nhap_kho",
                column: "ma_ncc");

            migrationBuilder.CreateIndex(
                name: "fk_pnk_nv",
                table: "phieu_nhap_kho",
                column: "ma_nguoi_lap");

            migrationBuilder.CreateIndex(
                name: "fk_pxk_dh",
                table: "phieu_xuat_kho",
                column: "ma_don_hang");

            migrationBuilder.CreateIndex(
                name: "fk_pxk_kho",
                table: "phieu_xuat_kho",
                column: "ma_kho_xuat");

            migrationBuilder.CreateIndex(
                name: "fk_sp_danhmuc",
                table: "san_pham",
                column: "ma_danh_muc");

            migrationBuilder.CreateIndex(
                name: "fk_sp_ncc",
                table: "san_pham",
                column: "ma_ncc_mac_dinh");

            migrationBuilder.CreateIndex(
                name: "ma_sku",
                table: "san_pham",
                column: "ma_sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_tk_kho",
                table: "the_kho",
                column: "ma_kho");

            migrationBuilder.CreateIndex(
                name: "fk_tk_sp",
                table: "the_kho",
                column: "ma_san_pham");

            migrationBuilder.CreateIndex(
                name: "fk_ton_sp",
                table: "ton_kho_chi_tiet",
                column: "ma_san_pham");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chi_tiet_don_hang");

            migrationBuilder.DropTable(
                name: "chi_tiet_phieu_nhap");

            migrationBuilder.DropTable(
                name: "cong_no_khach_hang");

            migrationBuilder.DropTable(
                name: "cong_no_ncc");

            migrationBuilder.DropTable(
                name: "don_vi_quy_doi");

            migrationBuilder.DropTable(
                name: "lichsuthanhtoan");

            migrationBuilder.DropTable(
                name: "phieu_xuat_kho");

            migrationBuilder.DropTable(
                name: "the_kho");

            migrationBuilder.DropTable(
                name: "ton_kho_chi_tiet");

            migrationBuilder.DropTable(
                name: "phieu_nhap_kho");

            migrationBuilder.DropTable(
                name: "don_hang");

            migrationBuilder.DropTable(
                name: "san_pham");

            migrationBuilder.DropTable(
                name: "kho");

            migrationBuilder.DropTable(
                name: "khach_hang");

            migrationBuilder.DropTable(
                name: "nguoi_dung");

            migrationBuilder.DropTable(
                name: "danh_muc");

            migrationBuilder.DropTable(
                name: "nha_cung_cap");
        }
    }
}
