using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class TapTinService : ITapTinService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; // Cần cái này để biết lưu file vào đâu

        public TapTinService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<TapTin> UploadFileAsync(IFormFile file, Guid? userId)
        {
            // 1. Kiểm tra file rỗng
            if (file == null || file.Length == 0)
                throw new Exception("File không hợp lệ.");

            // 2. Tạo tên file mới độc nhất (Tránh trường hợp 2 người cùng up file 'avatar.jpg')
            // Ví dụ: avatar.jpg -> avatar_GUID.jpg
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // 3. Xác định đường dẫn lưu trữ (Thư mục wwwroot/uploads)
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");

            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            // 4. Lưu file vật lý vào ổ cứng
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Lưu thông tin vào Database
            var tapTin = new TapTin
            {
                TapTinId = Guid.NewGuid(),
                // Đường dẫn tương đối để truy cập từ web
                DuongDan = $"/uploads/{uniqueFileName}",
                MimeType = file.ContentType,
                TaiBangNguoi = userId,
                ThoiGianTai = DateTimeOffset.Now
            };

            _context.TapTins.Add(tapTin);
            await _context.SaveChangesAsync();

            return tapTin;
        }

        public async Task<TapTin?> GetByIdAsync(Guid id)
        {
            return await _context.TapTins.FindAsync(id);
        }
    }
}
