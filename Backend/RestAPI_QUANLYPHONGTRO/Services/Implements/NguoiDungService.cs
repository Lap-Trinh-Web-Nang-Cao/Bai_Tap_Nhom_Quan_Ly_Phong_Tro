using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public NguoiDungService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            // 1. Kiểm tra Email trùng
            if (await _context.NguoiDungs.AnyAsync(u => u.Email == request.Email))
            {
                return false; // Email đã tồn tại
            }

            // 2. Tạo User mới
            var user = new NguoiDung
            {
                NguoiDungId = Guid.NewGuid(),
                Email = request.Email,
                DienThoai = request.DienThoai,
                VaiTroId = 3, // Mặc định Role = 3 (User thường)
                IsKhoa = false,
                IsEmailXacThuc = false,
                CreatedAt = DateTimeOffset.Now,

                // 3. MÃ HÓA MẬT KHẨU (Quan trọng)
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            // 1. Tìm user
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return null; // Sai Email

            // 2. Kiểm tra mật khẩu (So sánh hash)
            bool isValidPass = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isValidPass) return null; // Sai Pass

            // 3. Kiểm tra bị khóa
            if (user.IsKhoa) throw new Exception("Tài khoản đã bị khóa");

            // 4. Tạo JWT Token
            return GenerateJwtToken(user);
        }

        public async Task<NguoiDung?> GetByIdAsync(Guid id)
        {
            return await _context.NguoiDungs.FindAsync(id);
        }

        // Hàm phụ để tạo Token
        private string GenerateJwtToken(NguoiDung user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.NguoiDungId.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("VaiTroId", user.VaiTroId.ToString()) // Lưu Role ID để phân quyền
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _context.NguoiDungs.FindAsync(userId);
            if (user == null) return false;

            // Cập nhật thông tin
            user.DienThoai = request.DienThoai;
            user.UpdatedAt = DateTimeOffset.Now; // Ghi lại thời gian sửa

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _context.NguoiDungs.FindAsync(userId);
            if (user == null) return false;

            // B1: Kiểm tra mật khẩu cũ có đúng không
            bool isOldPassCorrect = BCrypt.Net.BCrypt.Verify(request.MatKhauCu, user.PasswordHash);
            if (!isOldPassCorrect)
            {
                return false; // Mật khẩu cũ sai
            }

            // B2: Mã hóa mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.MatKhauMoi);
            user.UpdatedAt = DateTimeOffset.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
