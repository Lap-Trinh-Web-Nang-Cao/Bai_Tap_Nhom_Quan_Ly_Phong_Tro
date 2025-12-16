using System.Configuration;

namespace ADMIN_QUANLYPHONGTRO.Models.Common
{
    public static class AppSettings
    {
        public static string ApiBaseUrl =>
            ConfigurationManager.AppSettings["ApiBaseUrl"];
    }
}
