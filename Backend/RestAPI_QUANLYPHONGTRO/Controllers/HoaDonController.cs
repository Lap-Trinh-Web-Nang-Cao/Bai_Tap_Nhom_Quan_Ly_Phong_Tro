using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HoaDonController : ControllerBase
    {
        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(id);
    }

   // GET /api/hoadon/nguoithue/{userId}
        [HttpGet("nguoithue/{userId}")]
        public async Task<IActionResult> GetByTenantId(Guid userId)
    {
     try
  {
 // TODO: Implement logic ð? l?y danh sách hóa ðõn c?a ngý?i thuê
         // T?m th?i tr? v? list r?ng
        return Ok(new { Success = true, Data = new List<object>(), Message = "L?y danh sách hóa ðõn thành công" });
 }
            catch (Exception ex)
            {
    return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // GET /api/hoadon - L?y t?t c? hóa ðõn
  [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
       {
                // TODO: Implement logic l?y t?t c? hóa ðõn
     return Ok(new { Success = true, Data = new List<object>(), Message = "L?y danh sách hóa ðõn thành công" });
            }
            catch (Exception ex)
       {
                return BadRequest(new { Success = false, Message = ex.Message });
      }
        }

        // GET /api/hoadon/{id}
      [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
     try
      {
         // TODO: Implement logic l?y 1 hóa ðõn by ID
  return Ok(new { Success = true, Data = (object)null, Message = "L?y hóa ðõn thành công" });
        }
  catch (Exception ex)
            {
   return BadRequest(new { Success = false, Message = ex.Message });
        }
        }

        // POST /api/hoadon/{id}/thanhtoan
        [HttpPost("{id}/thanhtoan")]
        public async Task<IActionResult> PayInvoice(Guid id)
        {
    try
        {
 // TODO: Implement logic thanh toán hóa ðõn
  return Ok(new { Success = true, Message = "Thanh toán hóa ðõn thành công" });
            }
            catch (Exception ex)
         {
           return BadRequest(new { Success = false, Message = ex.Message });
            }
    }
    }
}
