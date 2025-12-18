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
                // ===== STEP 1: QUERY =====
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

                // ===== STEP 3: COUNT =====
                var totalCount = await query.CountAsync();
                System.Diagnostics.Debug.WriteLine($"📊 Total count: {totalCount}");

                // ===== STEP 4: PAGINATION & MATERIALIZE =====
                var items = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Include(u => u.HoSoNguoiDung)  // ✅ JOIN HoSoNguoiDung
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine($"📄 Retrieved {items.Count} items after pagination");

                // ===== STEP 5: MAP TO DTO =====
                var dtoItems = items.Select(u => new HostPendingDto
                {
                    NguoiDungId = u.NguoiDungId,
                    HoTen = u.HoSoNguoiDung?.HoTen ?? "Chủ trọ",  // ✅ Get from HoSoNguoiDung
                    Email = u.Email ?? "",
                    DienThoai = u.DienThoai ?? "",
                    Avatar = "",  // ⚠️ Avatar không lưu trong HoSoNguoiDung, TODO: lấy từ TapTin
                    SoCCCD = u.HoSoNguoiDung?.LoaiGiayTo ?? "",  // ⚠️ Dùng LoaiGiayTo thay cho SoCCCD
                    LoaiGiayTo = u.HoSoNguoiDung?.LoaiGiayTo ?? "CCCD",
                    DaTaiGiayTo = false,  // TODO: Check TapTin table
                    SoTapTinDinhKem = 0,  // TODO: Count from TapTin
                    NgayDangKy = u.CreatedAt?.DateTime ?? DateTime.Now,
                    TrangThaiXacThuc = u.IsEmailXacThuc ? "Đã xác minh" : 
                                      u.IsKhoa ? "Từ chối" : "Chờ duyệt"  // ✅ Based on IsEmailXacThuc & IsKhoa
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"✅ Mapped {dtoItems.Count} DTOs");

                // ===== STEP 6: LOG SAMPLE =====
                if (dtoItems.Any())
                {
                    var sample = dtoItems.First();
                    System.Diagnostics.Debug.WriteLine($"📌 Sample Item:");
                    System.Diagnostics.Debug.WriteLine($"   - NguoiDungId: {sample.NguoiDungId}");
                    System.Diagnostics.Debug.WriteLine($"   - HoTen: {sample.HoTen}");
                    System.Diagnostics.Debug.WriteLine($"   - Email: {sample.Email}");
                    System.Diagnostics.Debug.WriteLine($"   - DienThoai: {sample.DienThoai}");
                    System.Diagnostics.Debug.WriteLine($"   - TrangThaiXacThuc: {sample.TrangThaiXacThuc}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️  No items to return!");
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
                    .Include(u => u.HoSoNguoiDung)  // ✅ JOIN HoSoNguoiDung
                    .FirstOrDefaultAsync(u => u.NguoiDungId == userId && u.VaiTroId == 2);

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Host not found: {hostId}");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"✅ Found host: {user.Email}");

                var dto = new HostApprovalDto
                {
                    NguoiDungId = user.NguoiDungId,
                    HoTen = user.HoSoNguoiDung?.HoTen ?? "Chủ trọ",  // ✅ Get from HoSoNguoiDung
                    Email = user.Email ?? "",
                    DienThoai = user.DienThoai ?? "",
                    SoCCCD = user.HoSoNguoiDung?.LoaiGiayTo ?? "",  // ⚠️ Use LoaiGiayTo
                    Avatar = "",  // ⚠️ Avatar not in HoSoNguoiDung
                    NgaySinh = user.HoSoNguoiDung?.NgaySinh ?? DateTime.Now,  // ✅ Get from HoSoNguoiDung
                    QueQuan = user.HoSoNguoiDung?.GhiChu ?? "",  // Use GhiChu as location
                    CCCDMatTruocUrl = "",  // TODO: Get from TapTin
                    CCCDMatSauUrl = "",  // TODO: Get from TapTin
                    GiayPhepKinhDoanhUrl = "",  // TODO: Get from TapTin
                    TrangThaiXacThuc = user.IsEmailXacThuc ? "Đã xác minh" : 
                                      user.IsKhoa ? "Từ chối" : "Chờ duyệt"
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

                // TODO: Update HoSoNguoiDung status to "Đã xác minh"
                // TODO: Send email notification to user

                await _context.SaveChangesAsync();
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

                // TODO: Update HoSoNguoiDung status to "Từ chối"
                // TODO: Store rejection reason
                // TODO: Send email notification to user with reason

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.RejectHostAsync Error: {ex.Message}");
                throw;
            }
        }
    }
}
