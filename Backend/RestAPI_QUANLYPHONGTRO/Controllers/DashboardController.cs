using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Dashboard API Controller - Cung cấp dữ liệu thống kê cho Admin
    /// Tất cả endpoints yêu cầu quyền Admin
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Lấy thống kê tổng quan Dashboard
        /// GET: api/dashboard/stats
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê phòng theo tháng
        /// GET: api/dashboard/rooms/monthly?months=12
        /// </summary>
        [HttpGet("rooms/monthly")]
        public async Task<IActionResult> GetMonthlyRoomStats([FromQuery] int months = 12)
        {
            try
            {
                if (months < 1 || months > 24)
                    return BadRequest(new { message = "Số tháng phải từ 1 đến 24" });

                var data = await _dashboardService.GetMonthlyRoomStatsAsync(months);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê theo tháng", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy phân bố trạng thái phòng
        /// GET: api/dashboard/rooms/status-distribution
        /// </summary>
        [HttpGet("rooms/status-distribution")]
        public async Task<IActionResult> GetRoomStatusDistribution()
        {
            try
            {
                var data = await _dashboardService.GetRoomStatusDistributionAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy phân bố trạng thái", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách phòng chờ duyệt (với pagination)
        /// GET: api/dashboard/rooms/pending?pageIndex=1&pageSize=10
        /// </summary>
        [HttpGet("rooms/pending")]
        public async Task<IActionResult> GetPendingRooms(
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageIndex < 1)
                    return BadRequest(new { message = "pageIndex phải >= 1" });
                if (pageSize < 1 || pageSize > 50)
                    return BadRequest(new { message = "pageSize phải từ 1 đến 50" });

                var data = await _dashboardService.GetPendingRoomsAsync(pageIndex, pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy phòng chờ duyệt", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy Top N phòng chờ duyệt (dùng cho Dashboard widget)
        /// GET: api/dashboard/rooms/pending-top?top=5
        /// </summary>
        [HttpGet("rooms/pending-top")]
        public async Task<IActionResult> GetTopPendingRooms([FromQuery] int top = 5)
        {
            try
            {
                if (top < 1 || top > 50)
                    return BadRequest(new { message = "Top phải từ 1 đến 50" });

                var data = await _dashboardService.GetTopPendingRoomsAsync(top);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy top phòng chờ duyệt", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy báo cáo vi phạm mới
        /// GET: api/dashboard/reports/recent?top=5
        /// </summary>
        [HttpGet("reports/recent")]
        public async Task<IActionResult> GetRecentReports([FromQuery] int top = 5)
        {
            try
            {
                if (top < 1 || top > 50)
                    return BadRequest(new { message = "Top phải từ 1 đến 50" });

                var data = await _dashboardService.GetRecentReportsAsync(top);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy báo cáo", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử hoạt động gần đây
        /// GET: api/dashboard/activities/recent?top=10
        /// </summary>
        [HttpGet("activities/recent")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int top = 10)
        {
            try
            {
                if (top < 1 || top > 100)
                    return BadRequest(new { message = "Top phải từ 1 đến 100" });

                var data = await _dashboardService.GetRecentActivitiesAsync(top);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy lịch sử hoạt động", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy số người dùng mới trong tháng
        /// GET: api/dashboard/users/new-this-month
        /// </summary>
        [HttpGet("users/new-this-month")]
        public async Task<IActionResult> GetNewUsersThisMonth()
        {
            try
            {
                var count = await _dashboardService.GetNewUsersThisMonthAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy số người dùng mới", error = ex.Message });
            }
        }
    }
}
