using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HopDongController : ControllerBase
    {
        private readonly IHostService _hostService;

   public HopDongController(IHostService hostService)
        {
         _hostService = hostService;
        }

 private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(id);
        }

        // GET /api/hopdong/nguoithue/{userId}/hieuluc
      [HttpGet("nguoithue/{userId}/hieuluc")]
        public async Task<IActionResult> GetActiveContractByTenantId(Guid userId)
        {
            try
    {
         // TODO: Implement logic ð? l?y h?p ð?ng ðang hi?u l?c c?a ngý?i thuê
  // T?m th?i tr? v? null (ngý?i thuê chýa có h?p ð?ng)
              return Ok(new { Success = true, Data = (object)null, Message = "Chýa có h?p ð?ng ðang hi?u l?c" });
            }
            catch (Exception ex)
    {
  return BadRequest(new { Success = false, Message = ex.Message });
         }
        }

        // GET /api/hopdong - L?y t?t c? h?p ð?ng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
          try
 {
    // TODO: Implement logic l?y t?t c? h?p ð?ng
        return Ok(new { Success = true, Data = new List<object>(), Message = "L?y danh sách h?p ð?ng thành công" });
       }
          catch (Exception ex)
{
            return BadRequest(new { Success = false, Message = ex.Message });
 }
     }

   // GET /api/hopdong/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
        {
  // TODO: Implement logic l?y 1 h?p ð?ng by ID
     return Ok(new { Success = true, Data = (object)null, Message = "L?y h?p ð?ng thành công" });
       }
         catch (Exception ex)
    {
           return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
