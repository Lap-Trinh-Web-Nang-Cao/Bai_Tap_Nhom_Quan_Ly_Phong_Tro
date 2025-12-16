using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class SupportsController : Controller
    {
        private readonly ISupportService _service;

        public SupportsController()
        {
            _service = new SupportService();
        }

        public async Task<ActionResult> Index(int page = 1, int pageSize = 10)
        {
            var data = await _service.GetSupportsAsync(page, pageSize);
            return View(data);
        }

        public async Task<ActionResult> Solve(string id)
        {
            await _service.MarkSolvedAsync(id);
            return RedirectToAction("Index");
        }
    }
}
