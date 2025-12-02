using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class HoSoNguoiDungService : IHoSoNguoiDungService
    {
        private readonly ApplicationDbContext _context;

        public HoSoNguoiDungService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HoSoNguoiDung?> GetProfileAsync(Guid userId)
        {
            return await _context.HoSoNguoiDungs.FindAsync(userId);
        }

        public async Task<HoSoNguoiDung> UpsertProfileAsync(Guid userId, HoSoNguoiDungRequest request)
        {
            var profile = await _context.HoSoNguoiDungs.FindAsync(userId);

            if (profile == null)
            {
                // -- TRƯỜNG HỢP TẠO MỚI --
                profile = new HoSoNguoiDung
                {
                    NguoiDungId = userId, // ID lấy từ Token
                    HoTen = request.HoTen,
                    NgaySinh = request.NgaySinh,
                    LoaiGiayTo = request.LoaiGiayTo,
                    GhiChu = request.GhiChu,
                    CreatedAt = DateTimeOffset.Now
                };
                _context.HoSoNguoiDungs.Add(profile);
            }
            else
            {
                // -- TRƯỜNG HỢP CẬP NHẬT --
                profile.HoTen = request.HoTen;
                profile.NgaySinh = request.NgaySinh;
                profile.LoaiGiayTo = request.LoaiGiayTo;
                profile.GhiChu = request.GhiChu;
                // Không cập nhật CreatedAt
            }

            await _context.SaveChangesAsync();
            return profile;
        }
    }
}
