using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Models;

namespace RestAPI_QUANLYPHONGTRO.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<BaoCaoViPham> BaoCaoViPhams { get; set; }
        public DbSet<BienLai> BienLais { get; set; }
        public DbSet<ChuTroThongTinPhapLy> ChuTroThongTinPhapLys { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<DanhGiaPhong> DanhGiaPhongs { get; set; }
        public DbSet<DatPhong> DatPhongs { get; set; }
        public DbSet<HanhDongAdmin> HanhDongAdmins { get; set; }
        public DbSet<HoSoNguoiDung> HoSoNguoiDungs { get; set; }
        public DbSet<LichSu> LichSus { get; set; }
        public DbSet<LoaiHoTro> LoaiHoTros { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }

        public DbSet<NguoiDungVaiTro> NguoiDungVaiTros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CẤU HÌNH COMPOSITE KEY ---
            modelBuilder.Entity<NguoiDungVaiTro>()
                .HasKey(nv => new { nv.NguoiDungId, nv.VaiTroId });
        }

        public DbSet<NhaTro> NhaTros { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<TienIch> TienIchs { get; set; }
        public DbSet<PhongTienIch> PhongTienIchs { get; set; }
        public DbSet<QuanHuyen> QuanHuyens { get; set; }
        public DbSet<Phuong> Phuongs { get; set; }
        public DbSet<TapTin> TapTins { get; set; }
        public DbSet<TinNhan> TinNhans { get; set; }
        public DbSet<TokenThongBao> TokenThongBaos { get; set; }

        public DbSet<TrangThaiDatPhong> TrangThaiDatPhongs { get; set; }
        public DbSet<ViPham> ViPhams { get; set; }
        public DbSet<YeuCauHoTro> YeuCauHoTros { get; set; }
    }
}
