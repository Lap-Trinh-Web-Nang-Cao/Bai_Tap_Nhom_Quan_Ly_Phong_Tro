using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System;
using System.Threading.Tasks;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostsController : ControllerBase
    {
        private readonly IHostService _service;

        public HostsController(IHostService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách chủ trọ chờ duyệt
        /// GET: api/hosts/pending?pageIndex=1&pageSize=10&keyword=
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingHosts(int pageIndex = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1 || pageSize > 50)
                    return BadRequest(new { message = "Invalid pagination parameters" });

                var result = await _service.GetPendingHostsAsync(pageIndex, pageSize, keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách chủ trọ", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy chi tiết chủ trọ để duyệt
        /// GET: api/hosts/{id}/detail
        /// </summary>
        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetHostDetail(string id)
        {
            try
            {
                var result = await _service.GetHostDetailAsync(id);
                if (result == null)
                    return NotFound(new { message = "Không tìm thấy chủ trọ" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy chi tiết chủ trọ", error = ex.Message });
            }
        }

        /// <summary>
        /// Phê duyệt chủ trọ
        /// PUT: api/hosts/{id}/approve
        /// </summary>
        [HttpPut("{id}/approve")]
        [Authorize]
        public async Task<IActionResult> ApproveHost(string id)
        {
            try
            {
                var result = await _service.ApproveHostAsync(id);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy chủ trọ" });

                return Ok(new { success = true, message = "Đã xác thực chủ trọ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xác thực chủ trọ", error = ex.Message });
            }
        }

        /// <summary>
        /// Từ chối chủ trọ
        /// PUT: api/hosts/{id}/reject
        /// </summary>
        [HttpPut("{id}/reject")]
        [Authorize]
        public async Task<IActionResult> RejectHost(string id, [FromBody] RejectHostRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Reason))
                    return BadRequest(new { message = "Vui lòng nhập lý do từ chối" });

                var result = await _service.RejectHostAsync(id, request.Reason);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy chủ trọ" });

                return Ok(new { success = true, message = "Đã từ chối chủ trọ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi từ chối chủ trọ", error = ex.Message });
            }
        }
    }
}
