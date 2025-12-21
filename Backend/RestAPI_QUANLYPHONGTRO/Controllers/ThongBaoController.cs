using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ThongBaoController : ControllerBase
    {
        private Guid GetUserId()
     {
    var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      return Guid.Parse(id);
        }

        // GET /api/thongbao/nguoithue/{userId}
        [HttpGet("nguoithue/{userId}")]
        public async Task<IActionResult> GetByTenantId(Guid userId)
        {
    try
            {
        // TODO: Implement logic ð? l?y danh sách thông báo c?a ngý?i thuê
          // T?m th?i tr? v? list r?ng
         return Ok(new { Success = true, Data = new List<object>(), Message = "L?y danh sách thông báo thành công" });
            }
  catch (Exception ex)
    {
      return BadRequest(new { Success = false, Message = ex.Message });
       }
        }

        // GET /api/thongbao - L?y t?t c? thông báo
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
    try
       {
         // TODO: Implement logic l?y t?t c? thông báo
            return Ok(new { Success = true, Data = new List<object>(), Message = "L?y danh sách thông báo thành công" });
            }
 catch (Exception ex)
  {
       return BadRequest(new { Success = false, Message = ex.Message });
   }
        }

  // GET /api/thongbao/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
  {
          try
            {
                // TODO: Implement logic l?y 1 thông báo by ID
  return Ok(new { Success = true, Data = (object)null, Message = "L?y thông báo thành công" });
            }
            catch (Exception ex)
            {
  return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // PUT /api/thongbao/{id}/daoc
        [HttpPut("{id}/daodoc")]
    public async Task<IActionResult> MarkAsRead(Guid id)
     {
         try
      {
    // TODO: Implement logic ðánh d?u ð? ð?c
     return Ok(new { Success = true, Message = "Ðánh d?u ð? ð?c thành công" });
 }
            catch (Exception ex)
 {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
