using System.Web;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
