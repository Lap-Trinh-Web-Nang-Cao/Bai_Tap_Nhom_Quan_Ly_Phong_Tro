using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class YeuCauHoTroService : IYeuCauHoTroService
    {
        private readonly ApplicationDbContext _context;

        public YeuCauHoTroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<YeuCauHoTro> CreateAsync(CreateYeuCauRequest request, Guid userId)
        {
            var yeuCau = new YeuCauHoTro
            {
                HoTroId = Guid.NewGuid(),
                PhongId = request.PhongId,
                LoaiHoTroId = request.LoaiHoTroId,
                TieuDe = request.TieuDe,
                MoTa = request.MoTa,
                NguoiYeuCau = userId,
                ThoiGianTao = DateTimeOffset.Now,

                // --- Logic nghiệp vụ ---
                TrangThai = "Moi" // Mặc định ban đầu
            };

            _context.YeuCauHoTros.Add(yeuCau);
            await _context.SaveChangesAsync();
            return yeuCau;
        }

        public async Task<IEnumerable<YeuCauHoTro>> GetMyRequestsAsync(Guid userId)
        {
            return await _context.YeuCauHoTros
                .Where(x => x.NguoiYeuCau == userId)
                .OrderByDescending(x => x.ThoiGianTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<YeuCauHoTro>> GetRequestsForLandlordAsync(Guid chuTroId)
        {
            // Join các bảng để tìm các Yêu cầu thuộc về Phòng -> thuộc về Nhà trọ -> của Chủ trọ này
            var query = from yc in _context.YeuCauHoTros
                        join p in _context.Phongs on yc.PhongId equals p.PhongId
                        join nt in _context.NhaTros on p.NhaTroId equals nt.NhaTroId
                        where nt.ChuTroId == chuTroId
                        orderby yc.ThoiGianTao descending
                        select yc;

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid hoTroId, string trangThaiMoi, Guid chuTroId)
        {
            var yeuCau = await _context.YeuCauHoTros.FindAsync(hoTroId);
            if (yeuCau == null) return false;

            // Kiểm tra quyền: Chỉ chủ trọ của phòng này mới được đổi trạng thái
            // (Query check xem phòng này có thuộc nhà trọ của ông này không)
            var isOwner = await _context.Phongs
                .Include(p => p.NhaTro)
                .AnyAsync(p => p.PhongId == yeuCau.PhongId && p.NhaTro.ChuTroId == chuTroId);

            if (!isOwner) throw new Exception("Bạn không có quyền xử lý yêu cầu này.");

            yeuCau.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();
            return true;
        }
        
        // === ADMIN METHODS ===
        
        public async Task<IEnumerable<YeuCauHoTro>> GetAllAsync()
        {
            return await _context.YeuCauHoTros
                .OrderByDescending(x => x.ThoiGianTao)
                .ToListAsync();
        }
        
        public async Task<YeuCauHoTro?> GetByIdAsync(Guid hoTroId)
        {
            return await _context.YeuCauHoTros.FindAsync(hoTroId);
        }
        
        public async Task<bool> AdminUpdateStatusAsync(Guid hoTroId, string trangThaiMoi)
        {
            var yeuCau = await _context.YeuCauHoTros.FindAsync(hoTroId);
            if (yeuCau == null) return false;
            
            yeuCau.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<SupportStatisticsDto> GetStatisticsAsync()
        {
            var allRequests = await _context.YeuCauHoTros.ToListAsync();
            
            return new SupportStatisticsDto
            {
                TotalRequests = allRequests.Count,
                NewRequests = allRequests.Count(x => x.TrangThai == "Moi"),
                ProcessingRequests = allRequests.Count(x => x.TrangThai == "DangXuLy"),
                CompletedRequests = allRequests.Count(x => x.TrangThai == "HoanThanh"),
                RejectedRequests = allRequests.Count(x => x.TrangThai == "TuChoi")
            };
        }
    }
}
