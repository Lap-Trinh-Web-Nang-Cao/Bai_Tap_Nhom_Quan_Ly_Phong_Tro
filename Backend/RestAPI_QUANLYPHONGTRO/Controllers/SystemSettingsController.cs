using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingService _service;

        public SystemSettingsController(ISystemSettingService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy tất cả cài đặt hệ thống
        /// </summary>
        [HttpGet]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var settings = await _service.GetAllSettingsAsync();
                return Ok(new { success = true, data = settings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy cài đặt theo nhóm
        /// </summary>
        [HttpGet("by-group/{groupName}")]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> GetByGroup(string groupName)
        {
            try
            {
                var settings = await _service.GetSettingsByGroupAsync(groupName);
                return Ok(new { success = true, data = settings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy cài đặt theo khóa
        /// </summary>
        [HttpGet("{key}")]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> GetByKey(string key)
        {
            try
            {
                var setting = await _service.GetSettingByKeyAsync(key);
                if (setting == null)
                    return NotFound(new { success = false, message = "Không tìm thấy cài đặt" });

                return Ok(new { success = true, data = setting });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Tạo cài đặt mới
        /// </summary>
        [HttpPost]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> Create([FromBody] SystemSettingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateSettingAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetByKey), new { key = request.SettingKey }, result);
        }

        /// <summary>
        /// Cập nhật cài đặt
        /// </summary>
        [HttpPut("{id}")]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> Update(int id, [FromBody] SystemSettingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateSettingAsync(id, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật cài đặt theo khóa (nhanh hơn)
        /// </summary>
        [HttpPost("update-by-key")]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> UpdateByKey([FromBody] Dictionary<string, string> request)
        {
            try
            {
                foreach (var kvp in request)
                {
                    await _service.UpdateSettingByKeyAsync(kvp.Key, kvp.Value);
                }

                return Ok(new { success = true, message = "Cập nhật cài đặt thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa cài đặt
        /// </summary>
        [HttpDelete("{id}")]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteSettingAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
