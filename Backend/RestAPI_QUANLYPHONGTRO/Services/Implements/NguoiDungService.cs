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

        /// <summary>
        /// Lấy danh sách users (phân trang) với filter
        /// </summary>
        public async Task<PagedResult<dynamic>> GetUsersAsync(int pageIndex, int pageSize, string keyword = "", int? vaiTroId = null, bool? isKhoa = null)
        {
            try
            {
                var query = _context.NguoiDungs.AsQueryable();

                // Filter by keyword
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var kw = keyword.ToLower();
                    query = query.Where(u =>
                        u.Email.ToLower().Contains(kw) ||
                        (u.DienThoai != null && u.DienThoai.Contains(kw)) ||
                        (u.HoSoNguoiDung != null && u.HoSoNguoiDung.HoTen != null && u.HoSoNguoiDung.HoTen.ToLower().Contains(kw))
                    );
                }

                // Filter by vai trò
                if (vaiTroId.HasValue && vaiTroId.Value > 0)
                {
                    query = query.Where(u => u.VaiTroId == vaiTroId.Value);
                }

                // Filter by trạng thái khóa
                if (isKhoa.HasValue)
                {
                    query = query.Where(u => u.IsKhoa == isKhoa.Value);
                }

                var totalCount = await query.CountAsync();

                // Pagination
                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Include(u => u.HoSoNguoiDung)
                    .ToListAsync();

                // Get VaiTro names
                var vaiTroIds = users.Select(u => u.VaiTroId).Distinct().ToList();
                var vaiTroMap = await _context.VaiTros
                    .Where(v => vaiTroIds.Contains(v.VaiTroId))
                    .ToDictionaryAsync(v => v.VaiTroId, v => v.TenVaiTro ?? "Unknown");

                // Map to anonymous object (không cần DTO)
                var displayList = users.Select(u => new
                {
                    u.NguoiDungId,
                    u.Email,
                    u.DienThoai,
                    HoTen = u.HoSoNguoiDung?.HoTen ?? "Chưa cập nhật",
                    u.VaiTroId,
                    VaiTroName = vaiTroMap.ContainsKey(u.VaiTroId) ? vaiTroMap[u.VaiTroId] : "Unknown",
                    u.IsEmailXacThuc,
                    u.IsKhoa,
                    CreatedAt = u.CreatedAt?.DateTime ?? DateTime.Now
                }).Cast<dynamic>().ToList();

                return new PagedResult<dynamic>
                {
                    Items = displayList,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetUsersAsync Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy chi tiết user (bao gồm hồ sơ)
        /// </summary>
        public async Task<dynamic> GetUserDetailAsync(Guid userId)
        {
            try
            {
                var user = await _context.NguoiDungs
                    .Include(u => u.HoSoNguoiDung)
                    .FirstOrDefaultAsync(u => u.NguoiDungId == userId);

                if (user == null) return null;

                // Lấy tên vai trò
                var vaiTro = await _context.VaiTros.FindAsync(user.VaiTroId);

                // Đếm số phòng đã đăng (nếu là chủ trọ)
                int soPhongDaDang = 0;
                if (user.VaiTroId == 2)
                {
                    soPhongDaDang = await _context.Phongs
                        .Where(p => p.NhaTro.ChuTroId == userId)
                        .CountAsync();
                }

                // Đếm số đặt phòng (nếu là người thuê)
                int soDatPhong = 0;
                if (user.VaiTroId == 3)
                {
                    soDatPhong = await _context.DatPhongs
                        .Where(d => d.NguoiThueId == userId)
                        .CountAsync();
                }

                return new
                {
                    user.NguoiDungId,
                    user.Email,
                    user.DienThoai,
                    HoTen = user.HoSoNguoiDung?.HoTen ?? "Chưa cập nhật",
                    NgaySinh = user.HoSoNguoiDung?.NgaySinh,
                    GhiChu = user.HoSoNguoiDung?.GhiChu ?? "",
                    LoaiGiayTo = user.HoSoNguoiDung?.LoaiGiayTo ?? "",
                    user.VaiTroId,
                    VaiTroName = vaiTro?.TenVaiTro ?? "Unknown",
                    user.IsEmailXacThuc,
                    user.IsKhoa,
                    CreatedAt = user.CreatedAt?.DateTime ?? DateTime.Now,
                    SoPhongDaDang = soPhongDaDang,
                    SoDatPhong = soDatPhong,
                    Avatar = "/Content/img/default-avatar.png"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetUserDetailAsync Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Admin tạo user mới
        /// </summary>
        public async Task<Guid?> CreateUserAsync(AdminCreateUserRequest request)
        {
            try
            {
                // 1. Kiểm tra Email trùng
                if (await _context.NguoiDungs.AnyAsync(u => u.Email == request.Email))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Email {request.Email} already exists");
                    return null; // Email đã tồn tại
                }

                // 2. Kiểm tra vai trò hợp lệ
                var vaiTro = await _context.VaiTros.FindAsync(request.VaiTroId);
                if (vaiTro == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Invalid VaiTroId: {request.VaiTroId}");
                    throw new Exception($"Vai trò không hợp lệ (ID: {request.VaiTroId})");
                }

                // 3. Tạo User mới
                var user = new NguoiDung
                {
                    NguoiDungId = Guid.NewGuid(),
                    Email = request.Email,
                    DienThoai = request.DienThoai,
                    VaiTroId = request.VaiTroId,
                    IsKhoa = false,
                    IsEmailXacThuc = request.IsEmailXacThuc,
                    CreatedAt = DateTimeOffset.Now,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                _context.NguoiDungs.Add(user);
                await _context.SaveChangesAsync();

                System.Diagnostics.Debug.WriteLine($"✅ User created: {user.Email}, ID: {user.NguoiDungId}");

                // 4. Tạo hồ sơ cơ bản (Optional - Nếu muốn)
                if (!string.IsNullOrWhiteSpace(request.HoTen))
                {
                    try
                    {
                        var hoSo = new HoSoNguoiDung
                        {
                            NguoiDungId = user.NguoiDungId,
                            HoTen = request.HoTen,
                            CreatedAt = DateTimeOffset.Now
                        };
                        _context.HoSoNguoiDungs.Add(hoSo);
                        await _context.SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine($"✅ HoSo created for: {user.Email}");
                    }
                    catch (Exception hoSoEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Warning creating HoSo: {hoSoEx.Message}");
                        // Không fail - User vẫn được tạo ngay cả khi HoSo fail
                    }
                }

                return user.NguoiDungId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreateUserAsync Error: {ex.Message}\n{ex.InnerException?.Message}");
                throw;
            }
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        public async Task<bool> LockUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.NguoiDungs.FindAsync(userId);
                if (user == null)
                    return false;

                user.IsKhoa = true;
                user.UpdatedAt = DateTimeOffset.Now;
                await _context.SaveChangesAsync();

                System.Diagnostics.Debug.WriteLine($"✅ User {user.Email} locked");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LockUserAsync Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        public async Task<bool> UnlockUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.NguoiDungs.FindAsync(userId);
                if (user == null)
                    return false;

                user.IsKhoa = false;
                user.UpdatedAt = DateTimeOffset.Now;
                await _context.SaveChangesAsync();

                System.Diagnostics.Debug.WriteLine($"✅ User {user.Email} unlocked");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UnlockUserAsync Error: {ex.Message}");
                throw;
            }
        }
    }
}
