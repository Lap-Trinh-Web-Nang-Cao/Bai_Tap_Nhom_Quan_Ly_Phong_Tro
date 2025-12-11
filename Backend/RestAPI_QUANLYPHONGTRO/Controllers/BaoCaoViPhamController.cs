using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoViPhamController : ControllerBase
    {
        private readonly IBaoCaoViPhamService _service;

        public BaoCaoViPhamController(IBaoCaoViPhamService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllBaoCaoAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetBaoCaoByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BaoCaoViPham baoCao)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdBaoCao = await _service.CreateBaoCaoAsync(baoCao);
            return CreatedAtAction(nameof(GetById), new { id = createdBaoCao.BaoCaoId }, createdBaoCao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BaoCaoViPham baoCao)
        {
            var updatedBaoCao = await _service.UpdateBaoCaoAsync(id, baoCao);
            if (updatedBaoCao == null) return NotFound();

            return Ok(updatedBaoCao);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _service.DeleteBaoCaoAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }
}
