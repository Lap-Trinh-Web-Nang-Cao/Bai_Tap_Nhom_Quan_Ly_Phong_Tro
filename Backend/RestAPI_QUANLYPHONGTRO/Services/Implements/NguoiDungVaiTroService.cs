using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class NguoiDungVaiTroService : INguoiDungVaiTroService
    {
        private readonly ApplicationDbContext _context;

        public NguoiDungVaiTroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddRoleToUserAsync(AssignRoleRequest request)
        {
            // 1. Kiểm tra xem user đã có vai trò này chưa
            var existing = await _context.NguoiDungVaiTros
                .FindAsync(request.NguoiDungId, request.VaiTroId);

            if (existing != null)
            {
                // Nếu đã có nhưng đang bị "NgayKetThuc" (đã hủy trước đó), thì kích hoạt lại
                if (existing.NgayKetThuc != null)
                {
                    existing.NgayKetThuc = null;
                    existing.NgayBatDau = DateTimeOffset.Now; // Reset ngày bắt đầu
                    existing.GhiChu = request.GhiChu;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false; // Đã tồn tại và đang hoạt động
            }

            // 2. Tạo mới
            var mapping = new NguoiDungVaiTro
            {
                NguoiDungId = request.NguoiDungId,
                VaiTroId = request.VaiTroId,
                NgayBatDau = DateTimeOffset.Now,
                NgayKetThuc = null,
                GhiChu = request.GhiChu
            };

            _context.NguoiDungVaiTros.Add(mapping);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(Guid userId, int roleId)
        {
            var item = await _context.NguoiDungVaiTros.FindAsync(userId, roleId);
            if (item == null) return false;

            // Cách 1: Xóa vĩnh viễn (Hard Delete)
            _context.NguoiDungVaiTros.Remove(item);

            // Cách 2: Chỉ cập nhật ngày kết thúc (Soft Delete - Lưu vết lịch sử)
            // item.NgayKetThuc = DateTimeOffset.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NguoiDungVaiTro>> GetRolesByUserIdAsync(Guid userId)
        {
            return await _context.NguoiDungVaiTros
                .Where(x => x.NguoiDungId == userId && x.NgayKetThuc == null) // Chỉ lấy các role đang hiệu lực
                                                                              // .Include(x => x.VaiTro) // Nếu muốn lấy tên vai trò thì mở comment này (cần config Navigation Property)
                .ToListAsync();
        }
    }
}
