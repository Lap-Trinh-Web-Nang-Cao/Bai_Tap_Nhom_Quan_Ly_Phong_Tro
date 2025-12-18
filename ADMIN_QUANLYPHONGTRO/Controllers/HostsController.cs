using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class HostsController : Controller
    {
        private readonly IHostService _service;

        public HostsController()
        {
            _service = new HostService();
        }

        // TODO: Implement when ChuTroThongTinPhapLyDto is defined
        // public async Task<ActionResult> Pending(int page = 1, int pageSize = 10)
        // {
        //     var data = await _service.GetPendingHostsAsync(page, pageSize);
        //     return View(data);
        // }

        public async Task<ActionResult> Approve(string id)
        {
            await _service.ApproveHostAsync(id);
            return RedirectToAction("Pending");
        }

        public async Task<ActionResult> Reject(string id, string reason)
        {
            await _service.RejectHostAsync(id, reason);
            return RedirectToAction("Pending");
        }
    }
}
