using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhuongController : ControllerBase
    {
        private readonly IPhuongService _service;

        public PhuongController(IPhuongService service)
        {
            _service = service;
        }

        // 1. Lấy tất cả (ít dùng, thường chỉ dùng cho Admin check data)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        // 2. Lấy Phường theo Quận (QUAN TRỌNG: Frontend sẽ gọi API này khi dropdown Quận thay đổi)
        // GET: api/phuong/by-quan/5
        [HttpGet("by-quan/{quanId}")]
        public async Task<IActionResult> GetByQuan(int quanId)
        {
            var result = await _service.GetByQuanIdAsync(quanId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // --- ADMIN ONLY ---

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PhuongRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.CreateAsync(request);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message); // Lỗi sai mã quận
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] PhuongRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
