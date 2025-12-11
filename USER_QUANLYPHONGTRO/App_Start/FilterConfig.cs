using System.Web;
using System.Web.Mvc;

namespace USER_QUANLYPHONGTRO
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
