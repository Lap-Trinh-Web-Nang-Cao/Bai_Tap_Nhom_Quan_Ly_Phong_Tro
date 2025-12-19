using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class HostService : IHostService
    {
        private readonly ApplicationDbContext _context;

        public HostService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách chủ trọ chờ duyệt
        /// </summary>
        public async Task<PagedResult<HostPendingDto>> GetPendingHostsAsync(int pageIndex, int pageSize, string keyword = "")
        {
            try
            {
                // ===== STEP 1: QUERY CHỦ TRỌ CHỜ DUYỆT =====
                var query = _context.NguoiDungs
                    .Where(u => u.VaiTroId == 2) // Role = Chủ trọ (VaiTroId = 2)
                    .AsQueryable();

                System.Diagnostics.Debug.WriteLine($"🔍 Initial query count: {query.Count()} users with VaiTroId=2");

                // ===== STEP 2: FILTER BY KEYWORD =====
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(u =>
                        u.Email.Contains(keyword) ||
                        u.DienThoai.Contains(keyword)
                    );
                    System.Diagnostics.Debug.WriteLine($"🔍 After keyword filter: {query.Count()} users");
                }

                // ===== STEP 3: COUNT TOTAL =====
                var totalCount = await query.CountAsync();
                System.Diagnostics.Debug.WriteLine($"📊 Total count: {totalCount}");

                // ===== STEP 4: PAGINATION & MATERIALIZE =====
                var items = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Include(u => u.HoSoNguoiDung)
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine($"📄 Retrieved {items.Count} items after pagination");

                // ===== STEP 5: GET RELATED DATA =====
                // Lấy thông tin pháp lý + tệp cho mỗi chủ trọ
                var nguoiDungIds = items.Select(u => u.NguoiDungId).ToList();

                var phapLyMap = await _context.ChuTroThongTinPhapLys
                    .Where(p => nguoiDungIds.Contains(p.NguoiDungId))
                    .ToDictionaryAsync(p => p.NguoiDungId, p => p);

                var tapTinIds = phapLyMap.Values
                    .Where(p => p.TapTinGiayToId.HasValue)
                    .Select(p => p.TapTinGiayToId.Value)
                    .Distinct()
                    .ToList();

                var tapTinMap = await _context.TapTins
                    .Where(t => tapTinIds.Contains(t.TapTinId))
                    .ToDictionaryAsync(t => t.TapTinId, t => t);

                // Đếm tệp cho mỗi người dùng (avatar, CCCD, v.v.)
                var fileCounts = await _context.TapTins
                    .Where(t => nguoiDungIds.Contains(t.TaiBangNguoi ?? Guid.Empty))
                    .GroupBy(t => t.TaiBangNguoi)
                    .Select(g => new { UserId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.UserId ?? Guid.Empty, x => x.Count);

                // ===== STEP 6: MAP TO DTO =====
                var dtoItems = items.Select(u =>
                {
                    var phapLy = phapLyMap.ContainsKey(u.NguoiDungId) ? phapLyMap[u.NguoiDungId] : null;
                    var tapTin = phapLy?.TapTinGiayToId.HasValue == true && 
                                 tapTinMap.ContainsKey(phapLy.TapTinGiayToId.Value)
                        ? tapTinMap[phapLy.TapTinGiayToId.Value]
                        : null;
                    
                    var soTapTin = fileCounts.ContainsKey(u.NguoiDungId) ? fileCounts[u.NguoiDungId] : 0;
                    var daTaiGiayTo = phapLy?.TapTinGiayToId.HasValue ?? false;

                    return new HostPendingDto
                    {
                        NguoiDungId = u.NguoiDungId,
                        HoTen = u.HoSoNguoiDung?.HoTen ?? "Chủ trọ",
                        Email = u.Email ?? "",
                        DienThoai = u.DienThoai ?? "",
                        Avatar = tapTin?.DuongDan ?? "", // Lấy từ TapTin nếu có
                        SoCCCD = phapLy?.CCCD ?? "", // Từ ChuTroThongTinPhapLy
                        LoaiGiayTo = u.HoSoNguoiDung?.LoaiGiayTo ?? "CCCD",
                        SoTapTinDinhKem = soTapTin,
                        DaTaiGiayTo = daTaiGiayTo,
                        NgayDangKy = u.CreatedAt?.DateTime ?? DateTime.Now,
                        TrangThaiXacThuc = GetHostStatus(u.IsEmailXacThuc, u.IsKhoa, phapLy?.TrangThaiXacThuc)
                    };
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"✅ Mapped {dtoItems.Count} DTOs");

                // ===== STEP 7: LOG SAMPLE =====
                if (dtoItems.Any())
                {
                    var sample = dtoItems.First();
                    System.Diagnostics.Debug.WriteLine($"📌 Sample Item:");
                    System.Diagnostics.Debug.WriteLine($"   - NguoiDungId: {sample.NguoiDungId}");
                    System.Diagnostics.Debug.WriteLine($"   - HoTen: {sample.HoTen}");
                    System.Diagnostics.Debug.WriteLine($"   - Email: {sample.Email}");
                    System.Diagnostics.Debug.WriteLine($"   - SoCCCD: {sample.SoCCCD}");
                    System.Diagnostics.Debug.WriteLine($"   - TrangThaiXacThuc: {sample.TrangThaiXacThuc}");
                }

                var result = new PagedResult<HostPendingDto>
                {
                    Items = dtoItems,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                System.Diagnostics.Debug.WriteLine($"✅ GetPendingHostsAsync completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.GetPendingHostsAsync Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Lấy chi tiết chủ trọ để duyệt
        /// </summary>
        public async Task<HostApprovalDto> GetHostDetailAsync(string hostId)
        {
            try
            {
                if (!Guid.TryParse(hostId, out var userId))
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Invalid Guid: {hostId}");
                    return null;
                }

                var user = await _context.NguoiDungs
                    .Include(u => u.HoSoNguoiDung)
                    .FirstOrDefaultAsync(u => u.NguoiDungId == userId && u.VaiTroId == 2);

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Host not found: {hostId}");
                    return null;
                }

                // Lấy thông tin pháp lý
                var phapLy = await _context.ChuTroThongTinPhapLys
                    .FirstOrDefaultAsync(p => p.NguoiDungId == userId);

                // Lấy các tệp liên quan
                var tapTins = await _context.TapTins
                    .Where(t => t.TaiBangNguoi == userId)
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine($"✅ Found host: {user.Email}");

                // Giả định: tệp có tên chứa CCCD_Mat_Truoc, CCCD_Mat_Sau, GiayPhep
                var cccdMatTruoc = tapTins.FirstOrDefault(t => 
                    t.DuongDan.Contains("CCCD", StringComparison.OrdinalIgnoreCase) && 
                    t.DuongDan.Contains("truoc", StringComparison.OrdinalIgnoreCase))?.DuongDan ?? "";
                
                var cccdMatSau = tapTins.FirstOrDefault(t => 
                    t.DuongDan.Contains("CCCD", StringComparison.OrdinalIgnoreCase) && 
                    t.DuongDan.Contains("sau", StringComparison.OrdinalIgnoreCase))?.DuongDan ?? "";
                
                var giayPhep = tapTins.FirstOrDefault(t => 
                    t.DuongDan.Contains("GiayPhep", StringComparison.OrdinalIgnoreCase) ||
                    t.DuongDan.Contains("HopDong", StringComparison.OrdinalIgnoreCase))?.DuongDan ?? "";

                var dto = new HostApprovalDto
                {
                    NguoiDungId = user.NguoiDungId,
                    HoTen = user.HoSoNguoiDung?.HoTen ?? "Chủ trọ",
                    Email = user.Email ?? "",
                    DienThoai = user.DienThoai ?? "",
                    SoCCCD = phapLy?.CCCD ?? "",
                    Avatar = tapTins.FirstOrDefault(t => 
                        t.DuongDan.Contains("avatar", StringComparison.OrdinalIgnoreCase) ||
                        t.MimeType?.StartsWith("image") == true)?.DuongDan ?? "",
                    NgaySinh = user.HoSoNguoiDung?.NgaySinh ?? DateTime.Now,
                    QueQuan = phapLy?.DiaChiThuongTru ?? "",
                    CCCDMatTruocUrl = cccdMatTruoc,
                    CCCDMatSauUrl = cccdMatSau,
                    GiayPhepKinhDoanhUrl = giayPhep,
                    TrangThaiXacThuc = GetHostStatus(user.IsEmailXacThuc, user.IsKhoa, phapLy?.TrangThaiXacThuc)
                };

                System.Diagnostics.Debug.WriteLine($"✅ Returning host detail: {dto.HoTen}");
                return dto;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.GetHostDetailAsync Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Phê duyệt chủ trọ
        /// </summary>
        public async Task<bool> ApproveHostAsync(string hostId)
        {
            try
            {
                if (!Guid.TryParse(hostId, out var userId))
                    return false;

                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.NguoiDungId == userId && u.VaiTroId == 2);

                if (user == null)
                    return false;

                // Cập nhật trạng thái người dùng
                user.IsEmailXacThuc = true;
                user.IsKhoa = false;
                user.UpdatedAt = DateTimeOffset.Now;

                // Cập nhật trạng thái xác thực pháp lý
                var phapLy = await _context.ChuTroThongTinPhapLys
                    .FirstOrDefaultAsync(p => p.NguoiDungId == userId);

                if (phapLy != null)
                {
                    phapLy.TrangThaiXacThuc = "DaDuyet";
                    phapLy.UpdatedAt = DateTimeOffset.Now;
                }

                await _context.SaveChangesAsync();

                // TODO: Gửi email thông báo duyệt cho chủ trọ
                System.Diagnostics.Debug.WriteLine($"✅ Host {user.Email} approved");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.ApproveHostAsync Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Từ chối chủ trọ
        /// </summary>
        public async Task<bool> RejectHostAsync(string hostId, string reason)
        {
            try
            {
                if (!Guid.TryParse(hostId, out var userId))
                    return false;

                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.NguoiDungId == userId && u.VaiTroId == 2);

                if (user == null)
                    return false;

                // Cập nhật trạng thái người dùng (có thể khóa hoặc để chờ)
                user.IsKhoa = true;
                user.UpdatedAt = DateTimeOffset.Now;

                // Cập nhật thông tin pháp lý
                var phapLy = await _context.ChuTroThongTinPhapLys
                    .FirstOrDefaultAsync(p => p.NguoiDungId == userId);

                if (phapLy != null)
                {
                    phapLy.TrangThaiXacThuc = "TuChoi";
                    phapLy.GhiChu = reason;
                    phapLy.UpdatedAt = DateTimeOffset.Now;
                }

                await _context.SaveChangesAsync();

                // TODO: Gửi email thông báo từ chối + lý do cho chủ trọ
                System.Diagnostics.Debug.WriteLine($"✅ Host {user.Email} rejected. Reason: {reason}");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.RejectHostAsync Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Helper: Xác định trạng thái xác thực chủ trọ
        /// </summary>
        private string GetHostStatus(bool isEmailVerified, bool isLocked, string? phapLyStatus)
        {
            if (isLocked)
                return "Đã từ chối";
            
            if (phapLyStatus == "DaDuyet" && isEmailVerified)
                return "Đã xác minh";
            
            if (phapLyStatus == "TuChoi")
                return "Từ chối";
            
            return "Chờ duyệt";
        }
    }
}
