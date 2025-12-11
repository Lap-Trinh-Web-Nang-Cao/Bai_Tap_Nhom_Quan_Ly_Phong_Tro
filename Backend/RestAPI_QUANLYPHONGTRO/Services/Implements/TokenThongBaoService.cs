using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class TokenThongBaoService : ITokenThongBaoService
    {
        private readonly ApplicationDbContext _context;

        public TokenThongBaoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterTokenAsync(Guid userId, string tokenString)
        {
            // 1. Kiểm tra xem token này đã tồn tại trong DB chưa
            var existingToken = await _context.TokenThongBaos
                .FirstOrDefaultAsync(x => x.Token == tokenString);

            if (existingToken != null)
            {
                // Nếu đã có: Cập nhật lại User sở hữu (phòng trường hợp 1 máy đăng nhập nhiều nick)
                // và bật Active lại
                existingToken.NguoiDungId = userId;
                existingToken.IsActive = true;
                existingToken.ThoiGianTao = DateTimeOffset.Now; // Update time
            }
            else
            {
                // Nếu chưa có: Tạo mới
                var newToken = new TokenThongBao
                {
                    TokenId = Guid.NewGuid(),
                    NguoiDungId = userId,
                    Token = tokenString,
                    ThoiGianTao = DateTimeOffset.Now,
                    IsActive = true
                };
                _context.TokenThongBaos.Add(newToken);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTokenAsync(string tokenString)
        {
            var token = await _context.TokenThongBaos
                .FirstOrDefaultAsync(x => x.Token == tokenString);

            if (token == null) return false;

            // Cách 1: Xóa vĩnh viễn (Khuyên dùng để nhẹ DB)
            _context.TokenThongBaos.Remove(token);

            // Cách 2: Soft delete (Chỉ tắt Active)
            // token.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
