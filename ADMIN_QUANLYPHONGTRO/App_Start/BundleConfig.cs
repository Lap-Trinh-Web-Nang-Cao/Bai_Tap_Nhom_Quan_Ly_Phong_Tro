using System.Web;
using System.Web.Optimization;

namespace ADMIN_QUANLYPHONGTRO
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ====== JQUERY BUNDLES ======
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/core/jquery.3.2.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // ====== BOOTSTRAP BUNDLES ======
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/core/popper.min.js",
                "~/Scripts/core/bootstrap.min.js"));

            // ====== ATLANTIS ADMIN TEMPLATE ======
            bundles.Add(new ScriptBundle("~/bundles/atlantis").Include(
                "~/Scripts/atlantis.min.js"));

            // ====== CUSTOM ADMIN SCRIPTS ======
            bundles.Add(new ScriptBundle("~/bundles/customadmin").Include(
                "~/Scripts/custom-admin.js"));

            // ====== CSS BUNDLES ======
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/atlantis.min.css",
                "~/Content/css/custom-admin.css"));

            // Disable optimization để dễ debug
            BundleTable.EnableOptimizations = false;
        }
    }
}
