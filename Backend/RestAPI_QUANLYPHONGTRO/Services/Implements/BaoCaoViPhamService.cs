using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class BaoCaoViPhamService : IBaoCaoViPhamService
    {
        private readonly ApplicationDbContext _context;

        public BaoCaoViPhamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BaoCaoViPham>> GetAllBaoCaoAsync()
        {
            return await _context.BaoCaoViPhams.ToListAsync();
        }

        public async Task<BaoCaoViPham?> GetBaoCaoByIdAsync(Guid id)
        {
            return await _context.BaoCaoViPhams.FindAsync(id);
        }

        public async Task<BaoCaoViPham> CreateBaoCaoAsync(BaoCaoViPham baoCao)
        {
            // Gán ID mới nếu chưa có
            if (baoCao.BaoCaoId == Guid.Empty)
            {
                baoCao.BaoCaoId = Guid.NewGuid();
            }

            // Gán thời gian báo cáo mặc định nếu null
            if (baoCao.ThoiGianBaoCao == null)
            {
                baoCao.ThoiGianBaoCao = DateTimeOffset.Now;
            }

            _context.BaoCaoViPhams.Add(baoCao);
            await _context.SaveChangesAsync();
            return baoCao;
        }

        public async Task<BaoCaoViPham?> UpdateBaoCaoAsync(Guid id, BaoCaoViPham baoCao)
        {
            var existingBaoCao = await _context.BaoCaoViPhams.FindAsync(id);
            if (existingBaoCao == null) return null;

            // Cập nhật các trường cần thiết
            existingBaoCao.LoaiThucThe = baoCao.LoaiThucThe;
            existingBaoCao.ThucTheId = baoCao.ThucTheId;
            existingBaoCao.NguoiBaoCao = baoCao.NguoiBaoCao;
            existingBaoCao.ViPhamId = baoCao.ViPhamId;
            existingBaoCao.TieuDe = baoCao.TieuDe;
            existingBaoCao.MoTa = baoCao.MoTa;
            existingBaoCao.TrangThai = baoCao.TrangThai;
            existingBaoCao.KetQua = baoCao.KetQua;
            existingBaoCao.NguoiXuLy = baoCao.NguoiXuLy;
            existingBaoCao.ThoiGianBaoCao = baoCao.ThoiGianBaoCao;
            existingBaoCao.ThoiGianXuLy = baoCao.ThoiGianXuLy;
            existingBaoCao.SoBaoCao = baoCao.SoBaoCao;

            await _context.SaveChangesAsync();
            return existingBaoCao;
        }

        public async Task<bool> DeleteBaoCaoAsync(Guid id)
        {
            var baoCao = await _context.BaoCaoViPhams.FindAsync(id);
            if (baoCao == null) return false;

            _context.BaoCaoViPhams.Remove(baoCao);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
