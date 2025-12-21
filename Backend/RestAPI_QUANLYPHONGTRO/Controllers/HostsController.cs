using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System;
using System.Linq;
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
        /// GET: api/hosts/pending?pageIndex=1&pageSize=10&keyword=&status=
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingHosts(
            int pageIndex = 1, 
            int pageSize = 10, 
            string keyword = "",
            string status = "")
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔍 Backend HostsController.GetPendingHosts: pageIndex={pageIndex}, pageSize={pageSize}");
                
                // Validate pagination - ensure pageIndex >= 1
                if (pageIndex < 1)
                    pageIndex = 1;
                
                if (pageSize < 1 || pageSize > 100)
                    return BadRequest(new { message = "Invalid pageSize. Must be between 1 and 100" });

                var result = await _service.GetPendingHostsAsync(pageIndex, pageSize, keyword);
                
                System.Diagnostics.Debug.WriteLine($"📦 Backend result: Total={result?.TotalCount}, Items={result?.Items?.Count}");
                
                // Filter by status if provided
                if (!string.IsNullOrEmpty(status) && result?.Items != null)
                {
                    var filteredItems = result.Items;
                    
                    switch (status.ToLower())
                    {
                        case "pending":
                            filteredItems = result.Items
                                .Where(x => x.TrangThaiXacThuc == "Chờ duyệt" || string.IsNullOrEmpty(x.TrangThaiXacThuc))
                                .ToList();
                            break;
                        case "approved":
                            filteredItems = result.Items
                                .Where(x => x.TrangThaiXacThuc == "Đã xác minh")
                                .ToList();
                            break;
                        case "rejected":
                            filteredItems = result.Items
                                .Where(x => x.TrangThaiXacThuc == "Từ chối" || x.TrangThaiXacThuc == "Đã từ chối")
                                .ToList();
                            break;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"✅ Filtered by status '{status}': {filteredItems.Count} items");
                    
                    return Ok(new
                    {
                        items = filteredItems,
                        pageIndex = result.PageIndex,
                        pageSize = result.PageSize,
                        totalCount = filteredItems.Count
                    });
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Returning {result?.Items?.Count ?? 0} items");
                
                return Ok(new
                {
                    items = result?.Items ?? new List<HostPendingDto>(),
                    pageIndex = result?.PageIndex ?? pageIndex,
                    pageSize = result?.PageSize ?? pageSize,
                    totalCount = result?.TotalCount ?? 0
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Backend HostsController.GetPendingHosts Error: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách chủ trọ", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê số lượng chủ trọ theo trạng thái
        /// GET: api/hosts/stats
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetHostStats()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("📊 Backend GetHostStats - Fetching data...");
                
                // Lấy tất cả chủ trọ để đếm - ensure pageIndex >= 1
                var result = await _service.GetPendingHostsAsync(1, 1000, "");
                
                var items = result?.Items ?? new List<HostPendingDto>();
                
                System.Diagnostics.Debug.WriteLine($"📊 Total hosts retrieved: {items.Count}");
                
                var pending = items.Count(x => 
                    x.TrangThaiXacThuc == "Chờ duyệt" || 
                    string.IsNullOrEmpty(x.TrangThaiXacThuc));
                var approved = items.Count(x => x.TrangThaiXacThuc == "Đã xác minh");
                var rejected = items.Count(x => 
                    x.TrangThaiXacThuc == "Từ chối" || 
                    x.TrangThaiXacThuc == "Đã từ chối");
                
                System.Diagnostics.Debug.WriteLine($"✅ Stats: Pending={pending}, Approved={approved}, Rejected={rejected}, Total={items.Count}");
                
                return Ok(new
                {
                    pending = pending,
                    approved = approved,
                    rejected = rejected,
                    total = items.Count
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Backend GetHostStats Error: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê", error = ex.Message });
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
        public async Task<IActionResult> ApproveHost(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest(new { success = false, message = "ID không hợp lệ" });

                var result = await _service.ApproveHostAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Không tìm thấy chủ trọ hoặc không thể xác thực" });

                return Ok(new { success = true, message = "Đã xác thực chủ trọ thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xác thực chủ trọ", error = ex.Message });
            }
        }

        /// <summary>
        /// Từ chối chủ trọ
        /// PUT: api/hosts/{id}/reject
        /// </summary>
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectHost(string id, [FromBody] RejectHostRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest(new { success = false, message = "ID không hợp lệ" });

                if (string.IsNullOrWhiteSpace(request?.Reason))
                    return BadRequest(new { success = false, message = "Vui lòng nhập lý do từ chối" });

                var result = await _service.RejectHostAsync(id, request.Reason);
                if (!result)
                    return NotFound(new { success = false, message = "Không tìm thấy chủ trọ hoặc không thể từ chối" });

                return Ok(new { success = true, message = "Đã từ chối chủ trọ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi từ chối chủ trọ", error = ex.Message });
            }
        }
    }
}
