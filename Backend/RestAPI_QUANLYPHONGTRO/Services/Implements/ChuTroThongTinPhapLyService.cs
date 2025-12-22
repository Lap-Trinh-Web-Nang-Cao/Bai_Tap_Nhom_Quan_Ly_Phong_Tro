using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class ChuTroThongTinPhapLyService : IChuTroThongTinPhapLyService
    {
        private readonly ApplicationDbContext _context;

        public ChuTroThongTinPhapLyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChuTroThongTinPhapLy>> GetAllAsync()
        {
            return await _context.ChuTroThongTinPhapLys.ToListAsync();
        }

        public async Task<ChuTroThongTinPhapLy?> GetByIdAsync(Guid nguoiDungId)
        {
            return await _context.ChuTroThongTinPhapLys.FindAsync(nguoiDungId);
        }

        public async Task<ChuTroThongTinPhapLy> CreateOrUpdateAsync(ChuTroThongTinPhapLy model)
        {
            var existing = await _context.ChuTroThongTinPhapLys.FindAsync(model.NguoiDungId);

            if (existing == null)
            {
                // -- TRƯỜNG HỢP TẠO MỚI --
                // Gán thời gian tạo
                model.CreatedAt = DateTimeOffset.Now;
                // Mặc định trạng thái ban đầu
                if (string.IsNullOrEmpty(model.TrangThaiXacThuc))
                {
                    model.TrangThaiXacThuc = "ChoDuyet";
                }

                _context.ChuTroThongTinPhapLys.Add(model);
            }
            else
            {
                // -- TRƯỜNG HỢP CẬP NHẬT --
                existing.CCCD = model.CCCD;
                existing.NgayCapCCCD = model.NgayCapCCCD;
                existing.NoiCapCCCD = model.NoiCapCCCD;
                existing.DiaChiThuongTru = model.DiaChiThuongTru;
                existing.DiaChiLienHe = model.DiaChiLienHe;
                existing.SoDienThoaiLienHe = model.SoDienThoaiLienHe;
                existing.MaSoThueCaNhan = model.MaSoThueCaNhan;
                existing.SoTaiKhoanNganHang = model.SoTaiKhoanNganHang;
                existing.TenNganHang = model.TenNganHang;
                existing.ChiNhanhNganHang = model.ChiNhanhNganHang;
                existing.TapTinGiayToId = model.TapTinGiayToId;
                existing.TrangThaiXacThuc = model.TrangThaiXacThuc;
                existing.GhiChu = model.GhiChu;


                // Cập nhật thời gian sửa đổi
                existing.UpdatedAt = DateTimeOffset.Now;                
                
            }

            await _context.SaveChangesAsync();
            return existing ?? model;
        }

        public async Task<bool> DeleteAsync(Guid nguoiDungId)
        {
            var item = await _context.ChuTroThongTinPhapLys.FindAsync(nguoiDungId);
            if (item == null) return false;

            _context.ChuTroThongTinPhapLys.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveAsync(Guid nguoiDungId, string trangThai)
        {
            var item = await _context.ChuTroThongTinPhapLys.FindAsync(nguoiDungId);
            if (item == null) return false;

            item.TrangThaiXacThuc = trangThai; // Ví dụ: "DaDuyet", "TuChoi"
            item.UpdatedAt = DateTimeOffset.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
