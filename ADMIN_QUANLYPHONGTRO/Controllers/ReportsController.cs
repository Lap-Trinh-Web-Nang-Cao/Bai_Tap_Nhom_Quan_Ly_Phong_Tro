using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService _service;

        public ReportsController()
        {
            _service = new ReportService();
        }

        public async Task<ActionResult> Index(int page = 1, int pageSize = 10)
        {
            var data = await _service.GetReportsAsync(page, pageSize);
            return View(data);
        }

        public async Task<ActionResult> Resolve(string id)
        {
            await _service.ResolveReportAsync(id);
            return RedirectToAction("Index");
        }
    }
}
