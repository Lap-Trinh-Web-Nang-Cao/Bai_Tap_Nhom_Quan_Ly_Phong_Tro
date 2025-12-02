using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BienLaiController : ControllerBase
    {
        private readonly IBienLaiService _service;

        public BienLaiController(IBienLaiService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BienLai bienLai)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdItem = await _service.CreateAsync(bienLai);
            return CreatedAtAction(nameof(GetById), new { id = createdItem.BienLaiId }, createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BienLai bienLai)
        {
            var updatedItem = await _service.UpdateAsync(id, bienLai);
            if (updatedItem == null) return NotFound();

            return Ok(updatedItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> Confirm(Guid id, [FromQuery] Guid nguoiXacNhanId)
        {
            // Kiểm tra đầu vào
            if (nguoiXacNhanId == Guid.Empty)
            {
                return BadRequest("ID người xác nhận không hợp lệ.");
            }

            var result = await _service.ConfirmBienLaiAsync(id, nguoiXacNhanId);

            if (!result)
            {
                return NotFound("Không tìm thấy biên lai với ID này.");
            }

            return Ok(new { message = "Xác nhận biên lai thành công!" });
        }
    }
}
