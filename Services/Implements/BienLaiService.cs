using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class BienLaiService : IBienLaiService
    {
        private readonly ApplicationDbContext _context;

        public BienLaiService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BienLai>> GetAllAsync()
        {
            return await _context.BienLais.ToListAsync();
        }

        public async Task<BienLai?> GetByIdAsync(Guid id)
        {
            return await _context.BienLais.FindAsync(id);
        }

        public async Task<BienLai> CreateAsync(BienLai bienLai)
        {
            // Logic tạo mới ID nếu chưa có
            if (bienLai.BienLaiId == Guid.Empty)
            {
                bienLai.BienLaiId = Guid.NewGuid();
            }

            // Logic gán thời gian tải lên mặc định là hiện tại nếu null
            if (bienLai.ThoiGianTai == null)
            {
                bienLai.ThoiGianTai = DateTimeOffset.Now;
            }

            // Mặc định chưa xác nhận khi mới tạo
            if (!bienLai.DaXacNhan)
            {
                bienLai.DaXacNhan = false;
            }

            _context.BienLais.Add(bienLai);
            await _context.SaveChangesAsync();
            return bienLai;
        }

        public async Task<BienLai?> UpdateAsync(Guid id, BienLai bienLai)
        {
            var existingItem = await _context.BienLais.FindAsync(id);
            if (existingItem == null) return null;

            // Cập nhật thông tin
            existingItem.DatPhongId = bienLai.DatPhongId;
            existingItem.NguoiTai = bienLai.NguoiTai;
            existingItem.TapTinId = bienLai.TapTinId;
            existingItem.SoTien = bienLai.SoTien;
            existingItem.ThoiGianTai = bienLai.ThoiGianTai;
            existingItem.DaXacNhan = bienLai.DaXacNhan;
            existingItem.NguoiXacNhan = bienLai.NguoiXacNhan;
            existingItem.SoBienLai = bienLai.SoBienLai;

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _context.BienLais.FindAsync(id);
            if (item == null) return false;

            _context.BienLais.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmBienLaiAsync(Guid id, Guid nguoiXacNhanId)
        {
            // 1. Tìm biên lai trong database
            var bienLai = await _context.BienLais.FindAsync(id);

            // 2. Kiểm tra nếu không tồn tại
            if (bienLai == null) return false;

            // 3. Thực hiện nghiệp vụ xác nhận
            bienLai.DaXacNhan = true;
            bienLai.NguoiXacNhan = nguoiXacNhanId;

            // 4. Lưu thay đổi
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
