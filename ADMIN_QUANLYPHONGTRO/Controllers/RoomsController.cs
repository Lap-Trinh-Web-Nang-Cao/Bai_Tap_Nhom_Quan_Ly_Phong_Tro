using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _service;

        public RoomsController()
        {
            _service = new RoomService();
        }

        public async Task<ActionResult> Pending(int page = 1, int pageSize = 10)
        {
            var data = await _service.GetPendingRoomsAsync(page, pageSize);
            return View(data);
        }

        public async Task<ActionResult> Approve(string id)
        {
            await _service.ApproveRoomAsync(id);
            return RedirectToAction("Pending");
        }

        public async Task<ActionResult> Reject(string id, string reason)
        {
            await _service.RejectRoomAsync(id, reason);
            return RedirectToAction("Pending");
        }

        public async Task<ActionResult> ToggleLock(string id)
        {
            await _service.ToggleLockRoomAsync(id);
            return RedirectToAction("Pending");
        }
    }
}
