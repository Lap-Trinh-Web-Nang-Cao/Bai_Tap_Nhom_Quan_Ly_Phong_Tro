using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _service;

        public TransactionsController()
        {
            _service = new TransactionService();
        }

        public async Task<ActionResult> Index(int page = 1, int pageSize = 10)
        {
            var data = await _service.GetTransactionsAsync(page, pageSize);
            return View(data);
        }

        public async Task<ActionResult> Confirm(string id)
        {
            await _service.ConfirmPaymentAsync(id);
            return RedirectToAction("Index");
        }
    }
}
