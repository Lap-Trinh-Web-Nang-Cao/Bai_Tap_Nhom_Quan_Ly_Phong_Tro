using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _service;

        public UsersController()
        {
            _service = new UserService();
        }

        public async Task<ActionResult> Index(int page = 1, int pageSize = 10, string keyword = "")
        {
            var data = await _service.GetUsersAsync(page, pageSize, keyword);
            ViewBag.Keyword = keyword;
            return View(data);
        }

        public async Task<ActionResult> Details(string id)
        {
            var result = await _service.GetUserByIdAsync(id);
            if (!result.Success) return HttpNotFound();
            return View(result.Data);
        }

        public async Task<ActionResult> ToggleLock(string id)
        {
            var result = await _service.ToggleLockUserAsync(id);
            return RedirectToAction("Index");
        }
    }
}
