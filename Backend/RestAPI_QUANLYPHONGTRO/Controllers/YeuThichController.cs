using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class YeuThichController : ControllerBase
    {
        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(id);
        }

        // GET /api/yeuthich/nguoithue/{userId}
        [HttpGet("nguoithue/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByTenantId(Guid userId)
        {
    try
   {
   // TODO: Implement logic ð? l?y danh sách ph?ng yêu thích c?a ngý?i thuê
 // T?m th?i tr? v? list r?ng
      return Ok(new { Success = true, Data = new List<object>(), Message = "L?y danh sách ph?ng yêu thích thành công" });
  }
         catch (Exception ex)
   {
    return BadRequest(new { Success = false, Message = ex.Message });
   }
        }

        // GET /api/yeuthich - L?y t?t c? yêu thích
        [HttpGet]
        [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        {
            try
            {
   // TODO: Implement logic l?y t?t c? yêu thích
      return Ok(new { Success = true, Data = new List<object>(), Message = "L?y danh sách yêu thích thành công" });
          }
    catch (Exception ex)
 {
         return BadRequest(new { Success = false, Message = ex.Message });
}
        }

        // POST /api/yeuthich/toggle
  [HttpPost("toggle")]
  [AllowAnonymous]
    public async Task<IActionResult> ToggleFavorite([FromBody] ToggleFavoriteRequest request)
     {
        try
            {
   // TODO: Implement logic toggle yêu thích
  return Ok(new { Success = true, Message = "C?p nh?t yêu thích thành công" });
    }
      catch (Exception ex)
 {
            return BadRequest(new { Success = false, Message = ex.Message });
     }
        }

        // POST /api/yeuthich
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
        {
  try
 {
   // TODO: Implement logic thêm yêu thích
                return Ok(new { Success = true, Message = "Thêm yêu thích thành công" });
      }
            catch (Exception ex)
         {
 return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // DELETE /api/yeuthich/{id}
        [HttpDelete("{id}")]
        [AllowAnonymous]
     public async Task<IActionResult> RemoveFavorite(Guid id)
        {
    try
            {
      // TODO: Implement logic xóa yêu thích
                return Ok(new { Success = true, Message = "Xóa yêu thích thành công" });
 }
            catch (Exception ex)
  {
       return BadRequest(new { Success = false, Message = ex.Message });
        }
        }
 }

    // DTO
    public class ToggleFavoriteRequest
    {
   public Guid PhongId { get; set; }
    public Guid NguoiThueId { get; set; }
 }

    public class AddFavoriteRequest
    {
        public Guid PhongId { get; set; }
        public Guid NguoiThueId { get; set; }
    }
}
